using ImageMagick;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;

namespace WallTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //MagickNET.Initialize("Magick.NET.net40.7.3.0.0");
            FixLensDistortion(new MagickImage(@"G0020020_1516279250023_high.JPG")).Write(@"finallagrange2.JPG");

            var img1 = new Mat(@"capture.PNG", ImreadModes.GrayScale);
            var detector = SURF.Create(hessianThreshold: 400); //A good default value could be from 300 to 500, depending from the image contrast.
            Log("Ready.");
            Console.Read();
        }

        private static MagickImage FixLensDistortion(MagickImage img)
        {
            Log("Starting lens correction.");
            img.Quality = 100;
            img.FilterType = FilterType.Lagrange;
            Log("Sharpening image.");
            img.AdaptiveSharpen();
            Log("Extending image.");
            img.Extent(new MagickGeometry(new Percentage(125), new Percentage(125)), Gravity.Center);
            Log("Barrel distortion removal.");
            img.Distort(DistortMethod.Barrel, new[] { -0.92578, 1.3845, -2.09958 });
            Log("Correcting perspective.");
            img.Distort(DistortMethod.Perspective, new double[] { 1095, 1560, 0, 0, 3394, 1558, 2300, 0, 3030, 2188, 2300, 1000, 1561, 2286, 0, 1000 });
            Log("Doing image crop.");
            img.Extent(2300, 1000);
            return img;
        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
