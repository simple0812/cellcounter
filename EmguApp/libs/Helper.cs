using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace EmguApp.libs
{
    public static class Helper
    {
        public static void WriteText(Image img, string text )
        {
            var image = new Mat(100, 400, DepthType.Cv8U, 3);
            image.SetTo(new Bgr(255, 255, 255).MCvScalar);
            CvInvoke.PutText(image, text, new Point(0, 50), FontFace.HersheyPlain, 3.0, new Bgr(255.0, 0.0, 0.0).MCvScalar);
            img.Source = BitmapSourceConvert.ToBitmapSource(image);
        }

        public static void RerverseColor(Mat src)
        {
            CvInvoke.BitwiseNot(src, src);
        }

        public static int Otsu(Mat src)
        {
            int height = src.Rows;
            int width = src.Cols;

            var x = src.ToImage<Gray, byte>();
            var  histogram = new float[256];
            for (int i = 0; i < x.Rows; i++)
                for (int j = 0; j < x.Cols; j++)
                {
                    var p = x[i, j];
                    histogram[(int)p.Intensity]++;
                }

            int size = height * width;
            for (int i = 0; i < 256; i++)
            {
                histogram[i] = histogram[i] / size;
            }

            float avgValue = 0;

            for (int i = 0; i < 256; i++)
            {
                avgValue += i * histogram[i];  //整幅图像的平均灰度 
            }

            int threshold = 0;
            float maxVariance = 0;
            float w = 0, u = 0;

            for (int i = 0; i < 256; i++)
            {
                w += histogram[i];  //假设当前灰度i为阈值, 0~i 灰度的像素(假设像素值在此范围的像素叫做前景像素) 所占整幅图像的比例 
                u += i * histogram[i];  // 灰度i 之前的像素(0~i)的平均灰度值： 前景像素的平均灰度值 

                float t = avgValue * w - u;
                float variance = t * t / (w * (1 - w));

                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    threshold = i;
                }
            }

            return threshold;
        }


        public static void FillHole(Mat srcBw, Mat dstBw)
        {
            try
            {
                Size m_Size = srcBw.Size;
                Mat Temp = new Mat(m_Size.Height + 2, m_Size.Width + 2, DepthType.Cv8U, 3);//延展图像
                srcBw.CopyTo(new Mat(Temp, new Rectangle(1, 1, m_Size.Width, m_Size.Height)));
                Mat cutImg = srcBw.Clone();//裁剪延展的图像

                new Mat(Temp, new Rectangle(1, 1, m_Size.Width, m_Size.Height)).CopyTo(cutImg);

                CvInvoke.BitwiseNot(cutImg, cutImg);
                CvInvoke.BitwiseOr(srcBw, cutImg, dstBw);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
        }

        public static Image<Gray, byte> FillHolesx(Image<Gray, byte> image)
        {
            var resultImage = image.CopyBlank();
            Gray gray = new Gray(255);

            var contoursDetected = new VectorOfVectorOfPoint();

            //CvInvoke.Canny(imgBinary, imgBinary, len, 255, 5, true);
            CvInvoke.FindContours(image, contoursDetected, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < contoursDetected.Size; i++)
            {
                resultImage.Draw(contoursDetected, i, new Gray(255), -1);
            }

            return resultImage;
        }

        public static BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }

            return returnSource;
        }

        public static Mat RemoveBlackBg(Mat src, double th = 30)
        {
            var x = src.ToImage<Bgr, byte>();
            for (int i = 0; i < x.Rows; i++)
            {
                for (int j = 0; j < x.Cols; j++)
                {
                    var p = x[i, j];

                    double r = p.Red;
                    double g = p.Green;
                    double b = p.Blue;

                    if (r <= th && g <= th && b <= th)
                    {
                        x[i, j] = new Bgr(255, 255, 255);
                    }
                }
            }

            return x.Mat.Clone();
        }

        public static Mat CovertBgFromWhiteToBlack(Mat src)
        {

            var x = src.ToImage<Bgr, byte>();
            for (int i = 0; i < x.Rows; i++)
            {
                for (int j = 0; j < x.Cols; j++)
                {
                    var p = x[i, j];

                    double r = p.Red;
                    double g = p.Green;
                    double b = p.Blue;

                    if (r >= 250 && g >= 250 && b >= 250)
                    {
                        x[i, j] = new Bgr(0, 0, 0);
                    }
                }
            }

            return x.Mat.Clone();
        }

        public static Mat DistanceTransform(Mat binary, double threshold = 0.5)
        {

            var dst = binary.Clone();
            CvInvoke.DistanceTransform(binary, dst, null, DistType.L2, 3);
            CvInvoke.Normalize(dst, dst, 1, 0, NormType.MinMax);

            CvInvoke.Threshold(dst, dst, threshold, 255, ThresholdType.Binary);

            Mat dist_8u = binary.Clone();
            dst.ConvertTo(dist_8u, DepthType.Cv8U);

            var contoursDetected = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(dist_8u, contoursDetected, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            Mat markers = new Mat(binary.Size, DepthType.Cv32S, 1);
            var x = markers.ToImage<Gray, byte>();
            x.SetZero();

            for (int i = 0; i < contoursDetected.Size; i++)
                CvInvoke.DrawContours(x, contoursDetected, i, new MCvScalar(255, 255, 255), -1);

            return x.Mat.Clone();
        }

        public static int FindCounters(Mat imgBinary)
        {
            try
            {
                var contoursDetected = new VectorOfVectorOfPoint();

                //CvInvoke.Canny(imgBinary, imgBinary, len, 255, 5, true);
                CvInvoke.FindContours(imgBinary, contoursDetected, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);
                Mat markers = new Mat(imgBinary.Size, DepthType.Cv32S, 3);

                for (int i = 0; i < contoursDetected.Size; i++)
                    CvInvoke.DrawContours(markers, contoursDetected, i, new MCvScalar(255), -1);

                return contoursDetected.Size;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);   
            }

            return 0;
        }
    }
}
