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
            FixLensDistortion(new MagickImage(@"input.JPG")).Write(@"wall.jpg");

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
           
            img.Write(@"wall_0.jpg");
            Log("Extending image.");
            img.Extent(new MagickGeometry(new Percentage(125), new Percentage(125)), Gravity.Center);
            img.Write(@"wall_1.jpg");
            Log("Barrel distortion removal.");
            img.Distort(DistortMethod.Barrel, new[] { -0.92578, 1.3845, -2.09958 });
            img.Write(@"wall_2.jpg");
            Log("Correcting perspective.");
            double w = 2300, h = 1000;
            /*                                                     top left           top right          bottom right      bottom left*/
            img.Distort(DistortMethod.Perspective, new double[] { 1072, 1512, 0, 0, 3404, 1524, w, 0, 3100, 2220, w, h, 1484, 2344, 0, h });
            img.Write(@"wall_3.jpg");
            Log("Doing image crop.");
            img.Extent(2300, 1000);

            Log("Sharpening image.");
            img.AdaptiveSharpen();
            return img;
        }

        private static void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
