using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private bool isMouseDown = false;
        private System.Windows.Point startPoint ;
        private Line line;
        public MainWindow()
        {
            InitializeComponent();
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

                canvas.Width = img.Width = src.Width;
                canvas.Height = img.Height = src.Height;
                img.Source = BitmapSourceConvert.ToBitmapSource(src);
            }

            var lines = canvas.FindChildren<Line>();

            foreach (var each in lines)
            {
                canvas.Children.Remove(each);
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
            workingImg = binary = Helper.FillHoles(binary.ToImage<Gray, byte>()).Mat.Clone();
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
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            Mat kernel1 = new Mat(3, 3, DepthType.Cv8U, 1);
            CvInvoke.Erode(workingImg, workingImg, kernel1, System.Drawing.Point.Empty, 1, BorderType.Default, new MCvScalar(0));
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
        }

        private void Dilate_OnClick(object sender, RoutedEventArgs e)
        {
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }

            Mat kernel1 = new Mat(3, 3, DepthType.Cv8U, 1);
            CvInvoke.Dilate(workingImg, workingImg, kernel1, System.Drawing.Point.Empty, 1, BorderType.Default, new MCvScalar(0));
            img.Source = BitmapSourceConvert.ToBitmapSource(workingImg);
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

            var tuple = Helper.FindCounters(workingImg);
            sbi3.Content = $"数量:{tuple.Item1}, 最小面积：{tuple.Item2}, 最大面积：{tuple.Item3}, 平均面积：{tuple.Item4}";

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

        private void Mark_OnClick(object sender, RoutedEventArgs e)
        {
            if (workingImg == null)
            {
                sbi1.Content = "请先选择图片";
                return;
            }
        }

        private void Img_OnMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if(!isMouseDown) return;

            var endPoint = Mouse.GetPosition(e.Source as FrameworkElement);
           
            line.X2 = endPoint.X;
            line.Y2 = endPoint.Y;
           
        }

        private void Img_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            isMouseDown = true;
            startPoint = Mouse.GetPosition(e.Source as FrameworkElement);

            line = new Line();
            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            line.X2 = startPoint.X;
            line.Y2 = startPoint.Y;
            line.Stroke = new SolidColorBrush(Colors.Orange);
            canvas.Children.Add(line);

        }

        private void Img_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            isMouseDown = false;
        }

        private void Tj_OnClick(object sender, RoutedEventArgs e)
        {
            var lines = canvas.FindChildren<Line>().Select(p => Math.Sqrt((p.X2 - p.X1)*(p.X2 - p.X1) + (p.Y2 - p.Y1) *(p.Y2 - p.Y1))).ToList();
            var min = lines.Min();
            var max = lines.Max();
            var ava = lines.Average();

            sbi3.Content = $"数量:{lines.Count}, 最小面积：{Math.Round(min,2)}, 最大面积：{Math.Round(max, 2)}, 平均面积：{Math.Round(ava, 2)}";
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            PictureHelper.SaveCanvas(this, this.canvas, 96, "e:/xx.png");
        }
    }
}
