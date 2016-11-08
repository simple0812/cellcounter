using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
using EmguApp.libs;

namespace EmguApp.Controls
{
    /// <summary>
    /// Stat.xaml 的交互逻辑
    /// </summary>
    public partial class Stat : Window
    {
        private IList<double> list = null;
        private bool isExec = false;

        public Stat(IList<double> list)
        {
            InitializeComponent();
            this.list = list;
            Loaded += Stat_Loaded;
        }

        private void Stat_Loaded(object sender, RoutedEventArgs e)
        {
            lv.ItemsSource = list.Select((t, i) => new Foo(i + 1, t.ToString("##.###"))).ToList();

            var path = $"file:///{AppDomain.CurrentDomain.BaseDirectory}html/stat.html";
            wb.Navigate(new Uri(path, UriKind.Absolute));
            wb.AllowWebBrowserDrop = false;

            ObjectForScriptingHelper helper = new ObjectForScriptingHelper(this);
            wb.ObjectForScripting = helper;
            wb.DocumentCompleted += Wb_DocumentCompleted;
            wb.NewWindow += Wb_NewWindow;
        }

        private void Wb_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Wb_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if(isExec) return;
            isExec = true;
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(IList<double>));
            MemoryStream msObj = new MemoryStream();
            //将序列化之后的Json格式数据写入流中
            js.WriteObject(msObj, list);
            msObj.Position = 0;
            //从0这个位置开始读取流中的数据
            StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            Debug.WriteLine(json);

            wb.Document?.InvokeScript("showChart", new object[] {json});
        }
    }

    public class Foo
    {
        public int Id { get; set; }
        public string Data { get; set; }

        public Foo(int id, string data)
        {
            Id = id;
            Data = data;
        }
    }

}
