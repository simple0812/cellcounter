using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using EmguApp.Controls;
using EmguApp.libs;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace EmguApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Mat src, gray, binary, workingImg;
        private string fileName = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnFile_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();

            ofd.DefaultExt = "*.png";
            ofd.Filter = "png (*.png)|*.png|jpg (*.jpg)|*.jpg|gif (*.gif)|*.gif|bmp (*.bmp)|*.bmp|jpeg (*.jpeg)|*.jpeg";
            if (ofd.ShowDialog() == true)
            {
                fileName = ofd.FileName;
                workingImg = src = CvInvoke.Imread(ofd.FileName, LoadImageType.Unchanged);
                gray = null;
                binary = null;

                img.Width = src.Width;
                img.Height = src.Height;
                img.Source = BitmapSourceConvert.ToBitmapSource(src);
            }
        }

        private void Gray_OnClick(object sender, RoutedEventArgs e)
        {
            if (src == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }
            try
            {
                var x = src.ToImage<Gray, byte>();
                workingImg = gray = x.Mat.Clone();
                img.Source = BitmapSourceConvert.ToBitmapSource(gray);
            }
            catch (Exception ex)
            {
                sbi2.Content = ex.Message;
            }

        }

        private void Thresholding_OnClick(object sender, RoutedEventArgs e)
        {
            if (gray == null)
            {
                sbi1.Content = "二值化前，请先将图片转为灰度图";
                return;
            }

            binary = gray.Clone();
            var otsu = Helper.Otsu(gray);
            CvInvoke.Threshold(gray, binary, otsu, 255, ThresholdType.Binary);
            workingImg = binary;
            img.Source = BitmapSourceConvert.ToBitmapSource(binary);

            var trackBar = new TrackBar();
            trackBar.Action = "Thresholding";
            trackBar.ValueChanged += TrackBar_ValueChanged;
            trackBar.ShowDialog();
        }

        private void TrackBar_ValueChanged(object sender, double val)
        {
            var bar = sender as TrackBar;
            if (bar == null) return;

            switch (bar.Action)
            {
                case "Thresholding":
                    CvInvoke.Threshold(gray, binary, val, 255, ThresholdType.Binary);
                    img.Source = BitmapSourceConvert.ToBitmapSource(binary);
                    break;
                case "RemoveBlackBg":
                    var x = Helper.RemoveBlackBg(src, val);
                    img.Source = BitmapSourceConvert.ToBitmapSource(x);
                    break;

                case "DistanceTransform":
                    workingImg = Helper.DistanceTransform(binary, val/100);
                    img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
                    break;
                default: break;
            }

        }

        private void RemoveBlackBg_OnClick(object sender, RoutedEventArgs e)
        {
            if (src == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            workingImg = src = Helper.RemoveBlackBg(src);
            img.Source = BitmapSourceConvert.ToBitmapSource(src);

            var trackBar = new TrackBar();
            trackBar.Action = "RemoveBlackBg";
            trackBar.ValueChanged += TrackBar_ValueChanged;
            trackBar.ShowDialog();
        }

        private void Reverse_OnClick(object sender, RoutedEventArgs e)
        {

            if (binary == null)
            {
                sbi1.Content = "请先将图片二值化";
                return;
            }

            Helper.RerverseColor(binary);
            workingImg = binary;
            img.Source = BitmapSourceConvert.ToBitmapSource(binary);
        }

        private void FillHole_OnClick(object sender, RoutedEventArgs e)
        {
            if (binary == null)
            {
                sbi1.Content = "请先将图片二值化";
                return;
            }

            sbi2.Content = "";
            workingImg = binary = Helper.FillHolesx(binary.ToImage<Gray, byte>()).Mat.Clone();
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }

        //黑底
        private void Distance_OnClick(object sender, RoutedEventArgs e)
        {
            if (binary == null)
            {
                sbi1.Content = "请先将图片二值化";
                return;
            }

            try
            {
                workingImg = Helper.DistanceTransform(binary);
                img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);

                var trackBar = new TrackBar();
                trackBar.Action = "DistanceTransform";
                trackBar.ValueChanged += TrackBar_ValueChanged;
                trackBar.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
           
        }

        private void ConvertBgToBlack_OnClick(object sender, RoutedEventArgs e)
        {
            if (src == null)
            {
                sbi1.Content = "请选择图片";
                return;
            }

            workingImg = src = Helper.CovertBgFromWhiteToBlack(src);
            img.Source = BitmapSourceConvert.ToBitmapSource(src);
        }

        private void Erode_OnClick(object sender, RoutedEventArgs e)
        {
            if (binary == null)
            {
                sbi1.Content = "请先将图片二值化";
                return;
            }

            Mat kernel1 = new Mat(3, 3, DepthType.Cv8U, 1);
            CvInvoke.Erode(binary, binary, kernel1, System.Drawing.Point.Empty, 1, BorderType.Default, new MCvScalar(0));
            workingImg = binary;
            img.Source = BitmapSourceConvert.ToBitmapSource(binary);
        }

        private void Dilate_OnClick(object sender, RoutedEventArgs e)
        {
            if (binary == null)
            {
                sbi1.Content = "请先将图片二值化";
                return;
            }

            Mat kernel1 = new Mat(3, 3, DepthType.Cv8U, 1);
            CvInvoke.Dilate(binary, binary, kernel1, System.Drawing.Point.Empty, 1, BorderType.Default, new MCvScalar(0));
            workingImg = binary;
            img.Source = BitmapSourceConvert.ToBitmapSource(binary);
        }

        private void Blur_OnClick(object sender, RoutedEventArgs e)
        {
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            CvInvoke.Blur(workingImg, workingImg, new System.Drawing.Size(2, 2), new Point(-1, -1));
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }

        private void GaussianBlur_OnClick(object sender, RoutedEventArgs e)
        {
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            CvInvoke.GaussianBlur(workingImg, workingImg, new Size(1, 1), 200, 200);
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }

        private void MedianBlur_OnClick(object sender, RoutedEventArgs e)
        {
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            CvInvoke.MedianBlur(workingImg, workingImg, 3);
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }

        private void Count_OnClick(object sender, RoutedEventArgs e)
        {
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            var count = Helper.FindCounters(workingImg);
            sbi3.Content = count.ToString();

            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }

        private void SimpleBlob_OnClick(object sender, RoutedEventArgs e)
        {
            var param = new SimpleBlobDetectorParams();

            //            param.MinThreshold = 0;
            //            param.MaxThreshold = 1000;

            param.FilterByArea = true;
            param.MinArea = 1500;

            param.FilterByCircularity = true;
            param.MinCircularity = 0.1f;

            param.FilterByConvexity = true;
            param.MinConvexity = 0.87f;

            param.FilterByInertia = true;
            param.MinInertiaRatio = 1.2f;

            var blob = new SimpleBlobDetector();

            var points = blob.Detect(workingImg);
            sbi3.Content = points.Length.ToString();
            CvInvoke.Canny(workingImg, workingImg, Helper.Otsu(workingImg), 255, 3);
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }
    }
}
