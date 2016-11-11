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
using System.Windows.Shapes;

namespace EmguApp.Controls
{
    /// <summary>
    /// SetScale.xaml 的交互逻辑
    /// </summary>
    public partial class SetScale : Window
    {
        private double realLen;
        public SetScale(double len)
        {
            InitializeComponent();
            this.realLen = len;
        }

        public event Action<double, string> OnSuccess; 

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var p = cb.SelectionBoxItem.ToString(); 
            double len = double.TryParse(txtLen.Text, out len) ? len : 0;
            if (len > 0 && realLen > 0)
            {
                OnSuccess?.Invoke(len / realLen, p);
                this.Close();

            }
        }
    }
}
