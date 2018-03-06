using GoProRetrieve.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Encoder = System.Drawing.Imaging.Encoder;

namespace GoProRetrieve
{
    class Program
    {
        private static Timer _keepAlive;
        private static readonly object Transferring = new object(), KeepingCameraAlive = new object();

        private static void Log(string msg)
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

        // https://msdn.microsoft.com/en-us/library/ytz20d80%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == mimeType);
        }

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            var s = Stopwatch.StartNew();
            Log("Initializing");

            _keepAlive = new Timer(KeepCameraAlive);
            DoCameraKeepAliveNow();

            do
            {
                CaptureAndDownload(null);

                Log("Waiting now");
                Thread.Sleep(Math.Max(1,Settings.Default.CaptureEveryMinutes * 60 * 1000 - (int)s.ElapsedMilliseconds));
                s.Restart();
            } while (true);
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

                // Force a keep alive here
                DoCameraKeepAliveNow();

                var t = Stopwatch.StartNew();
                for (var i = 0; i < Settings.Default.CaptureTotalPhotos; i++)
                {
                    if (i > 0)
                        Thread.Sleep(Math.Max(1,
                            (int)(Settings.Default.CaptureWaitBetweenPhotoSeconds * 1000 -
                                   t.ElapsedMilliseconds))); // Wait for the next time

                    t.Restart();
                    SendCameraCommand(Settings.Default.CameraCapturePhoto,
                        $"Capture {i + 1} of {Settings.Default.CaptureTotalPhotos}");
                }

                // Retrieve file list
                Thread.Sleep(5000);

                // Force a keep alive here
                DoCameraKeepAliveNow();

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
                        /*RedirectStandardError = true,
                        RedirectStandardOutput = true*/
                    },
                };

                //averaging.OutputDataReceived += Averaging_OutputDataReceived;
                //averaging.ErrorDataReceived += Averaging_OutputDataReceived;

                averaging.Start();
                averaging.PriorityClass = ProcessPriorityClass.BelowNormal;
                //averaging.BeginOutputReadLine();
                //averaging.BeginErrorReadLine();
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
                        /*RedirectStandardError = true,
                        UseShellExecute = false,*/
                    });
                    comparison.PriorityClass = ProcessPriorityClass.BelowNormal;
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
                var now = DateTime.Now;
                var finalImageName = "wall_" + now.ToString("yyyyMMddHHmm") + ".jpg";
                var enhancedImageName = "enhanced-" + finalImageName;

                using (var img = new Bitmap(Settings.Default.FinalImageBeforeEnhancementFilename))
                {
                    img.Save(finalImageName, GetEncoderInfo("image/jpeg"),
                        new EncoderParameters {Param = {[0] = new EncoderParameter(Encoder.Quality, 95L)}});
                }

                var enhancement = Process.Start(Settings.Default.ImageEnhancementProgram, string.Format(Settings.Default.ImageEnhancementArgs,
                    Settings.Default.FinalImageBeforeEnhancementFilename, enhancedImageName));
                enhancement.PriorityClass = ProcessPriorityClass.BelowNormal;
                enhancement.WaitForExit();

                foreach (var image in new[] { finalImageName, enhancedImageName })
                {
                    Log("Uploading: " + image);

                    for (var i = 0; i < Settings.Default.CameraCommandsRetriesTotal; i++)
                        try
                        {
                            var wc = new TimeoutWebClient();
                            wc.Headers.Add("Content-Type", "binary/octet-stream");
                            wc.UploadFile(Settings.Default.FinalImageUploadServer, "POST", image);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Log("Upload failed: " + ex.Message);
                            Thread.Sleep(1000);
                            Log("Retrying ... ");
                        }
                }

                Log("All done");
            }
            catch (Exception ex)
            {
                // https://stackoverflow.com/questions/3328990/c-sharp-get-line-number-which-threw-exception
                var st = new StackTrace(ex, true);

                // Get the top stack frame
                var frame = st.GetFrames().Last();
                Log($"Error (in {frame.GetMethod().Name}:{frame.GetFileLineNumber()}) while running: {ex.Message}");
            }
        }

        private static void DoCameraKeepAliveNow()
        {
            _keepAlive.Change(Settings.Default.CameraKeepAliveEveryMinutes * 60 * 1000, Settings.Default.CameraKeepAliveEveryMinutes * 60 * 1000);
            KeepCameraAlive(null);
        }

        // https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed

        public static bool MyRemoteCertificateValidationCallback(Object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var isOk = true;
            // If there are errors in the certificate chain,
            // look at each error to determine the cause.

            try
            {
                if (sslPolicyErrors != SslPolicyErrors.None)
                {
                    foreach (var t in chain.ChainStatus)
                    {
                        if (t.Status == X509ChainStatusFlags.RevocationStatusUnknown) continue;

                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

                        if (chain.Build((X509Certificate2)certificate)) continue;

                        isOk = false;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Log("Error in certificate validation: " + ex.Message);
                isOk = false;
            }
            return isOk;
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

        private static void SendCameraCommand(string command, string message, out string response, bool retry = true)
        {
            lock (Transferring)
            {
                Log($"{message} ...");

                for (var i = 0; i < Settings.Default.CameraCommandsRetriesTotal; i++)
                    try
                    {
                        var wc = new TimeoutWebClient(Settings.Default.CameraCommandsTimeoutSeconds);
                        response = wc.DownloadString(command);

                        Log($"{message} -> {{{wc.ResponseHeaders}}},\"{response}\"".Replace("\n", string.Empty)
                            .Replace("\r", string.Empty));
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log($"{message} -> Error (attempt={i}): {ex.Message}");

                        if (!retry)
                            break;

                        // Wait for the next retry
                        Thread.Sleep(Settings.Default.CameraCommandsRetryTimeoutSeconds * 1000);
                    }

                response = null;
            }
        }

        private static void KeepCameraAlive(object state)
        {
            if (Monitor.TryEnter(KeepingCameraAlive))
            {
                // Keep camera alive
                // Sending wake on Lan
                var camera = PhysicalAddress.Parse(Settings.Default.CameraPhysicalAddress.ToUpper().Replace(':', '-'));
                camera.SendWol();

                // TODO: Check other IPs to broadcast the WOL

                SendCameraCommand(Settings.Default.CameraKeepAlive, "Keeping camera alive", out var s, false);
            }
            else
                Log("Skipping this keep alive message. There is one waiting already.");
        }
    }

    internal struct CameraPhoto
    {
        public string Path;
        public string ThumbnailPath;
    }
}