using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace WallMonitor
{
    class Program
    {
        private static FileSystemWatcher _monitor;
        private Stack<string> pendingFiles;

        static void Main(string[] args)
        {
            _monitor = new FileSystemWatcher("input");
            _monitor.Changed += Monitor_Changed;
            _monitor.EnableRaisingEvents = true;

            Console.WriteLine("Waiting for files");
            Thread.Sleep(Timeout.Infinite);
        }

        private static void Monitor_Changed(object sender, FileSystemEventArgs e)
        {
            if(IsFileReady(e.FullPath))
                Console.WriteLine("Changed: " + e.FullPath);
        }

        public static bool IsFileReady(String sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
