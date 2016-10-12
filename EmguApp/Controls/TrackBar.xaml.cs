using System;
using System.Windows;

namespace EmguApp.Controls
{
    /// <summary>
    /// TrackBar.xaml 的交互逻辑
    /// </summary>
    public partial class TrackBar : Window
    {

        public event Action<object, double> ValueChanged; 
        public string Action { get; set; }
      
        public TrackBar()
        {
            InitializeComponent();
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ValueChanged?.Invoke(this, e.NewValue);
        }
    }
}
