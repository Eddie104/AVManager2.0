using avManager.model;
using avManager.model.data;
using libra.web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AVManager2.avManager.view.video
{
    /// <summary>
    /// NewVideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewVideoWindow : Window
    {

        public Video CurrentVideo { get; set; }

        public NewVideoWindow()
        {
            InitializeComponent();
        }

        private void OnSearchVideoInfo(object sender, RoutedEventArgs e)
        {
            string code = this.codeTextBox.Text.ToUpper();
            if (Regex.IsMatch(code, @"[A-Z]+-[0-9]+"))
            {
                HTMLHelper.GetInstance().GetHtml(string.Format("http://www.javlibrary.com/cn/vl_searchbyid.php?keyword={0}", code), this.callback);
            }
            else
            {
                MessageBox.Show(string.Format("番号:{0}格式有误", code));
            }
        }

        private void callback(string html)
        {
            CurrentVideo = VideoManager.GetInstance().CreateVideo(html);
            //this.nameTextBox.Text = CurrentVideo.Name;
            //this.birthdayTextBox.Text = v.Date.ToString("YY-MM-DD");
            Console.WriteLine("name={0}", CurrentVideo.Name);

            this.nameTextBox.Dispatcher.Invoke(new Action(delegate { this.nameTextBox.Text = CurrentVideo.Name; }));
            this.birthdayTextBox.Dispatcher.Invoke(new Action(delegate { this.birthdayTextBox.Text = CurrentVideo.Date.ToString("YY-MM-DD"); }));
        }

        private void OnAddHandler(object sender, RoutedEventArgs e)
        {
            if (CurrentVideo != null)
            {
                VideoManager.GetInstance().AddVideo(CurrentVideo);
            }
            else
            {
                MessageBox.Show("没有可以添加的影片信息");
            }
        }

        private void OnCloseHandler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
