using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostItFinder
{
    public partial class FormMain : Form
    {

        private readonly string wall;
        private readonly string[] postit;

        public FormMain()
        {
            InitializeComponent();
            /*openFileDialogFile.Title = "Wall file";
            openFileDialogFile.ShowDialog();
            var wall = openFileDialogFile.FileName;


            openFileDialogFile.Title = "Post it";
            openFileDialogFile.ShowDialog();
            var postit = openFileDialogFile.FileName;*/

            /*var;
            var postit = @"D:\GitHub\DualogWall\rpi\WallTool\WallTool\bin\Debug\capture.PNG";

            //FindMatch(new Mat(wall), new Mat(postit), out long matchtime, out var keypoints, out var observer, out var matches, out var mask, out var homography);

            pictureBox1.Image = Draw(new Mat(wall), new Mat(postit), out var time).Bitmap;
            Text = "Took: " + time;*/

            /* openFileDialogFile.Title = "Wall file";
             openFileDialogFile.ShowDialog();
             wall = openFileDialogFile.FileName;


             openFileDialogFile.Title = "Post it";
             openFileDialogFile.ShowDialog();
             postit = openFileDialogFile.FileName;*/


            wall = @"D:\GitHub\DualogWall\rpi\WallTool\WallTool\bin\Debug\wallnew.jpg";

            openFileDialogFile.Title = "Post its";
            openFileDialogFile.Multiselect = true;
            openFileDialogFile.ShowDialog();
            postit = openFileDialogFile.FileNames;
            backgroundWorker1.RunWorkerAsync();

            //pictureBox1.Image = Draw(new Mat(wall), new Mat(postit), out var time, 0.8, 300).Bitmap;

        }

        public static void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography, double uniquenessThreshold, double hessianThresh)
        {
            int k = 2;
            int nonZero = 4;
            /*double uniquenessThreshold = 0.8;
            double hessianThresh = 300;*/

            Stopwatch watch;
            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();
/*
#if !__IOS__
            if (CudaInvoke.HasCuda)
            {
                CudaSURF surfCuda = new CudaSURF((float)hessianThresh);
                using (GpuMat gpuModelImage = new GpuMat(modelImage))
                //extract features from the object image
                using (GpuMat gpuModelKeyPoints = surfCuda.DetectKeyPointsRaw(gpuModelImage, null))
                using (GpuMat gpuModelDescriptors = surfCuda.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                using (CudaBFMatcher matcher = new CudaBFMatcher(DistanceType.L2))
                {
                    surfCuda.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    using (GpuMat gpuObservedImage = new GpuMat(observedImage))
                    using (GpuMat gpuObservedKeyPoints = surfCuda.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat gpuObservedDescriptors = surfCuda.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                    //using (GpuMat tmp = new GpuMat())
                    //using (Stream stream = new Stream())
                    {
                        matcher.KnnMatch(gpuObservedDescriptors, gpuModelDescriptors, matches, k);

                        surfCuda.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                        mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(255));
                        Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                        int nonZeroCount = CvInvoke.CountNonZero(mask);
                        if (nonZeroCount >= nonZero)
                        {
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                               matches, mask, 1.5, 20);
                            if (nonZeroCount >= nonZero)
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                                   observedKeyPoints, matches, mask, 2);
                        }
                    }
                    watch.Stop();
                }
            }
            else
#endif*/
            {
                using (UMat uModelImage = modelImage.GetUMat(Emgu.CV.CvEnum.AccessType.Read))
                using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
                {
                    SURF surfCPU = new SURF(hessianThresh);
                    //extract features from the object image
                    UMat modelDescriptors = new UMat();
                    try
                    {
                        surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e);
                        throw;
                    }
                   
                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    UMat observedDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                    BFMatcher matcher = new BFMatcher(DistanceType.L2);
                    matcher.Add(modelDescriptors);

                    matcher.KnnMatch(observedDescriptors, matches, k, null);
                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= nonZero)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                           matches, mask, 1.5, 20);
                        if (nonZeroCount >= nonZero)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                               observedKeyPoints, matches, mask, 2);
                    }

                    watch.Stop();
                }
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Draw the model image and observed image, the matched features and homography projection.
        /// </summary>
        /// <param name="modelImage">The model image</param>
        /// <param name="observedImage">The observed image</param>
        /// <param name="matchTime">The output total time for computing the homography matrix.</param>
        /// <returns>The model image and observed image, the matched features and homography projection.</returns>
        public static Mat Draw(Mat modelImage, Mat observedImage, out long matchTime,double u, double h, out bool doesItHasPolylines)
        {
            doesItHasPolylines = false;
            Mat homography;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindMatch(modelImage, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, matches,
                   out mask, out homography, u, h);


                //Draw the matched keypoints
                Mat result = new Mat();
                Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
                   matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

                #region draw the projected region on the image

                if (homography != null)
                {
                    doesItHasPolylines = true;

                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[]
                    {
                  new PointF(rect.Left, rect.Bottom),
                  new PointF(rect.Right, rect.Bottom),
                  new PointF(rect.Right, rect.Top),
                  new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);

                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    using (VectorOfPoint vp = new VectorOfPoint(points))
                    {
                        CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                        
                    }

                }

                #endregion

                return result;

            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
                for (Double uniquines = 0.95; uniquines <= 1; uniquines += 0.025)
                    for (Double hessian = 250; hessian <= 310; hessian += 10)
                    foreach (var p in postit)
                    {
                        //FindMatch(new Mat(wall), new Mat(postit), out long matchtime, out var keypoints, out var observer, out var matches, out var mask, out var homography);

                        // Save output
                        if (!Directory.Exists("output"))
                            Directory.CreateDirectory("output");



                        try
                        {
                            var img = Draw(new Mat(wall), new Mat(p), out var time, uniquines, hessian, out bool save).Bitmap;

                            backgroundWorker1.ReportProgress(0, Tuple.Create(time, (Bitmap)img.Clone(), uniquines + " - " + hessian));


                            if (save)
                                img.Save(Path.Combine("output", Path.GetFileNameWithoutExtension(p) + "_" + uniquines + "_" + hessian + ".png"));

                        }
                        catch (Exception exception)
                        {
                            System.Console.WriteLine(exception);
                        }
                       
                        

                        // Thread.Sleep(4000);
                    }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                var t = e.UserState as Tuple<long, Bitmap, string>;

                pictureBox1.Image = t.Item2;
                Text = "Done. Time: " + t.Item1 + " ms / Uniq/hessian: " + t.Item3;
            }
        }
    }
}
