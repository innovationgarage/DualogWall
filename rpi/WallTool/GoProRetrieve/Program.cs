using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            Console.WriteLine(msg);
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
            // Deleting all
            SendCameraCommand(Settings.Default.CameraDeleteMediaAll, "Deleting old files");

            // Do the captures
            var t = Stopwatch.StartNew();
            for (var i = 0; i < Settings.Default.CaptureTotalPhotos; i++)
            {
                if (i > 0)
                    Thread.Sleep(Math.Max(0,
                        (int) (Settings.Default.CaptureWaitBetweenPhotoSeconds * 1000 -
                               t.ElapsedMilliseconds))); // Wait for the next time

                t.Restart();
                SendCameraCommand(Settings.Default.CameraCapturePhoto,
                    $"Capture {i + 1} of {Settings.Default.CaptureTotalPhotos}");
            }

            // Retrieve file list
            SendCameraCommand(Settings.Default.CameraListMedia, "Listing files", out var jsonMediaList);

            // Download photos
            var photos = new List<string>();
            foreach (var element in JObject.Parse(jsonMediaList))
            {
                if (element.Key == "media")
                {
                    foreach (var sub in element.Value)
                        photos.AddRange(sub["fs"].Select(file => $"{sub["d"]}/{file["n"]}"));
                    break;
                }
            }

            foreach (var photo in photos)
                DownloadCameraPhoto(photo);

            // Lens correction
            /*Log("Doing lens correction");
            var p = Process.Start(Settings.Default.LensCorrectionProgram, Settings.Default.LensCorrectionArgs);
            p.WaitForExit();*/

            // Other enhancements
            Log("Enhancing image");
        }

        private static void DownloadCameraPhoto(string path)
        {
            Log("Downloading " + path);

            var attempts = Settings.Default.CameraCommandsRetriesTotal;

            var outputDirectory = "from_camera";

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            while (true)
                try
                {
                    new TimeoutWebClient(Settings.Default.CameraCommandsTimeout).
                        DownloadFile(string.Format(Settings.Default.CameraGetMedia,path),Path.Combine("from_camera",Path.GetFileName(path)));
                    Log("** OK " + path);
                    break;
                }
                catch (Exception ex)
                {
                    Log($"Error downloading {path} (remaining attempts={attempts}): {ex.Message}");

                    if (--attempts <= 0)
                        break;

                    // Wait for the next retry
                    Thread.Sleep(Settings.Default.CameraCommandsRetryTimeoutSeconds * 1000);
                }
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
                    var wc = new TimeoutWebClient(Settings.Default.CameraCommandsTimeout);
                    response = wc.DownloadString(command);

                    Log($"{message} -> {{{wc.ResponseHeaders}}},\"{response}\"".Replace("\n", string.Empty)
                        .Replace("\r", string.Empty));
                    break;
                }
                catch (Exception ex)
                {
                    Log($"{message} -> Error (remaining attempts={attempts}): {ex.Message}");

                    response = null;
                    if (--attempts <= 0)
                        break;

                    // Wait for the next retry
                    Thread.Sleep(Settings.Default.CameraCommandsRetryTimeoutSeconds * 1000);
                }
        }

        private static void KeepCameraAlive(object state)
        {
            // Keep camera alive
            SendCameraCommand(Settings.Default.CameraKeepAlive, "Keeping camera alive");
        }
    }

}