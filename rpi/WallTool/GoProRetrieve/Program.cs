using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoProRetrieve.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoProRetrieve
{
    class Program
    {
        private static Timer keepAlive;
        private static Timer downloadPictures;

        static void Log(string msg)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {msg}");
        }

        // https://stackoverflow.com/questions/1789627/how-to-change-the-timeout-on-a-net-webclient-object
        private class TimeoutWebClient : WebClient
        {
            private readonly int _timeout;

            public TimeoutWebClient(int timeout = 60)
            {
                _timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                var w = base.GetWebRequest(uri);
                w.Timeout = _timeout * 60 * 1000;
                return w;
            }
        }

        static void Main(string[] args)
        {
            Log("Initializing");

            keepAlive = new Timer(KeepCameraAlive, null, Settings.Default.CameraKeepAliveEveryMinutes * 60 * 1000,
                Settings.Default.CameraKeepAliveEveryMinutes * 60 * 1000);

            downloadPictures = new Timer(CaptureAndDownload, null, 0,
                Settings.Default.CaptureEveryMinutes * 60 * 1000);

            Thread.Sleep(Timeout.Infinite);
        }

        private static void CaptureAndDownload(object state)
        {
            try
            {
                Log("Preparing image enhancement in the background");
                Process.Start(Settings.Default.ImageEnhancementProgram, string.Format(Settings.Default.ImagePreEnhancementArgs));

                // Deleting all
                SendCameraCommand(Settings.Default.CameraDeleteMediaAll, "Deleting old files");

                if (Directory.Exists(Settings.Default.DirectoryCaptures))
                    Directory.Delete(Settings.Default.DirectoryCaptures, true);

                // Do the captures
                //SendCameraCommand(Settings.Default.CameraSetPhotoMode, "Switching to night mode");
                //SendCameraCommand(Settings.Default.CameraSetPhotoQuality, "Using highest quality");

                var t = Stopwatch.StartNew();
                for (var i = 0; i < Settings.Default.CaptureTotalPhotos; i++)
                {
                    if (i > 0)
                        Thread.Sleep(Math.Max(0,
                            (int)(Settings.Default.CaptureWaitBetweenPhotoSeconds * 1000 -
                                   t.ElapsedMilliseconds))); // Wait for the next time

                    t.Restart();
                    SendCameraCommand(Settings.Default.CameraCapturePhoto,
                        $"Capture {i + 1} of {Settings.Default.CaptureTotalPhotos}");
                }

                // Retrieve file list
                Thread.Sleep(5000);
                SendCameraCommand(Settings.Default.CameraListMedia, "Listing files", out var jsonMediaList);

                // Download photos
                var photos = new List<string>();
                foreach (var element in JObject.Parse(jsonMediaList))
                    if (element.Key == "media")
                    {
                        foreach (var sub in element.Value)
                            photos.AddRange(sub["fs"].Select(file => $"{sub["d"]}/{file["n"]}"));
                        break; // Only the first directory
                    }

                var downloaded = (from p in photos.Select(DownloadCameraPhoto) where p != null select p.Value).ToList();

                // Select the photo with the less chance of having people
                // TODO: Check for errors while running subprocesses
                Log("Averaging photos (This might take a while)");

                var averaging = new Process
                {
                    StartInfo = new ProcessStartInfo(Settings.Default.MergePhotosProgram,
                        string.Format(Settings.Default.MergePhotosArgs, string.Join(" ", downloaded.Select(i => i.ThumbnailPath)),
                        Settings.Default.CompareDifferencesTemporalFilename))
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    },
                };

                averaging.OutputDataReceived += Averaging_OutputDataReceived;
                averaging.ErrorDataReceived += Averaging_OutputDataReceived;

                averaging.Start();
                //averaging.PriorityClass = ProcessPriorityClass.BelowNormal;
                averaging.BeginOutputReadLine();
                averaging.BeginErrorReadLine();
                averaging.WaitForExit();

                Log("Comparing differences");
                double minimum = double.MaxValue;
                var selected = downloaded.First();

                foreach (var photo in downloaded)
                {
                    var comparison = Process.Start(new ProcessStartInfo(Settings.Default.CompareDifferencesProgram,
                        string.Format(Settings.Default.CompareDifferencesArgs, photo.ThumbnailPath, Settings.Default.CompareDifferencesTemporalFilename))
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                    });
                    comparison.WaitForExit();

                    //Log(comparison.StartInfo.FileName + " args:"+ comparison.StartInfo.Arguments);
                    var c = comparison.StandardOutput.ReadToEnd() + comparison.StandardError.ReadToEnd();
                    Log("Result: " + c);

                    var m = double.Parse(c.Split(' ')[0].Trim().Replace(".", new NumberFormatInfo().NumberDecimalSeparator));
                    Log($"Comparison {photo.Path} <=> {Settings.Default.CompareDifferencesTemporalFilename}: {m}");

                    if (!(minimum > m)) continue;

                    minimum = m;
                    selected = photo;
                }

                // Keeping the best
                Log("The best is " + selected.Path);

                // Lens correction
                Log("Doing lens correction and sharpening (This might also take a while)");
                var lens = Process.Start(Settings.Default.LensCorrectionProgram,
                    string.Format(Settings.Default.LensCorrectionArgs, selected.Path, Settings.Default.FinalImageBeforeEnhancementFilename));
                lens.WaitForExit();

                // Other enhancements
                Log("Enhancing image");
                Process.Start(Settings.Default.ImageEnhancementProgram, string.Format(Settings.Default.ImageEnhancementArgs,
                    Settings.Default.FinalImageBeforeEnhancementFilename, Settings.Default.FinalImageFilename)).WaitForExit();

                Log("Ready. Uploading: " + Settings.Default.FinalImageFilename);

                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "binary/octet-stream");
                wc.UploadFile(Settings.Default.FinalImageUploadServer, "POST", Settings.Default.FinalImageFilename);

                Log("All done");
            }
            catch(Exception ex)
            {
                // https://stackoverflow.com/questions/3328990/c-sharp-get-line-number-which-threw-exception
                var st = new StackTrace(ex, true);

                // Get the top stack frame
                var frame = st.GetFrames().Last();

                Log($"Error (in {frame.GetMethod().Name}:{frame.GetFileLineNumber()}) while running: {ex.Message}");
            }
        }

        private static void Averaging_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data.Trim()))
                return;

            Log(e.Data);
        }

        private static CameraPhoto? DownloadCameraPhoto(string path)
        {
            Log("Downloading " + path);

            var attempts = Settings.Default.CameraCommandsRetriesTotal;

            var outputDirectory = Settings.Default.DirectoryCaptures;
            var n = Path.GetFileName(path);
            var outputPhotoPath = Path.Combine(outputDirectory, n);
            var outputThumbPath = Path.Combine(outputDirectory, "thumb_" + n);

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            while (true)
                try
                {
                    new TimeoutWebClient(Settings.Default.CameraCommandsTimeoutSeconds).
                        DownloadFile(string.Format(Settings.Default.CameraGetMedia,path),outputPhotoPath);

                    // Make thumbnail
                    var bmp = Image.FromFile(outputPhotoPath);
                    bmp.GetThumbnailImage(bmp.Width / 4, bmp.Height / 4, null, IntPtr.Zero).Save(outputThumbPath);

                    Log("** OK " + path);
                    return new CameraPhoto { Path = outputPhotoPath, ThumbnailPath = outputThumbPath};
                }
                catch (Exception ex)
                {
                    Log($"Error downloading {path} (remaining attempts={attempts}): {ex.Message}");

                    if (--attempts < 0)
                        break;

                    // Wait for the next retry
                    Thread.Sleep(Settings.Default.CameraCommandsRetryTimeoutSeconds * 1000);
                }
            return null;
        }

        private static void SendCameraCommand(string command, string message)
        {
            SendCameraCommand(command, message, out var r);
        }

        private static void SendCameraCommand(string command, string message, out string response)
        {
            Log($"{message} ...");

            var attempts = Settings.Default.CameraCommandsRetriesTotal;

            while (true)
                try
                {
                    var wc = new TimeoutWebClient(Settings.Default.CameraCommandsTimeoutSeconds);
                    response = wc.DownloadString(command);

                    Log($"{message} -> {{{wc.ResponseHeaders}}},\"{response}\"".Replace("\n", string.Empty)
                        .Replace("\r", string.Empty));
                    break;
                }
                catch (Exception ex)
                {
                    Log($"{message} -> Error (remaining attempts={attempts}): {ex.Message}");

                    response = null;
                    if (--attempts < 0)
                        break;

                    // Wait for the next retry
                    Thread.Sleep(Settings.Default.CameraCommandsRetryTimeoutSeconds * 1000);
                }
        }

        private static void KeepCameraAlive(object state)
        {
            // Keep camera alive
            lock(keepAlive)
                SendCameraCommand(Settings.Default.CameraKeepAlive, "Keeping camera alive");
        }
    }

    internal struct CameraPhoto
    {
        public string Path;
        public string ThumbnailPath;
    }
}