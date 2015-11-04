using avManager.model;
using avManager.model.data;
using libra.web;
using Libra.helper;
using Libra.log4CSharp;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

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

            if (RegularHelper.IsUrl(CurrentVideo.ImgUrl))
            {
                this.coverImg.Dispatcher.Invoke(new Action(delegate { this.coverImg.Source = new BitmapImage(new Uri(CurrentVideo.ImgUrl)); }));
            }
            else
            {
                Logger.Error(string.Format("{0}的封面地址有误:{1}", CurrentVideo.Code, CurrentVideo.ImgUrl));
            }
            
            this.nameTextBox.Dispatcher.Invoke(new Action(delegate { this.nameTextBox.Text = CurrentVideo.Name; }));
            this.birthdayTextBox.Dispatcher.Invoke(new Action(delegate { this.birthdayTextBox.Text = CurrentVideo.Date.ToString("yyyy-MM-dd"); }));
            this.classTypeTextBox.Dispatcher.Invoke(new Action(delegate { this.classTypeTextBox.Text = CurrentVideo.GetClassString(); }));
            this.actressTextBox.Dispatcher.Invoke(new Action(delegate { this.actressTextBox.Text = CurrentVideo.GetActressString(); }));
        }

        private void OnAddHandler(object sender, RoutedEventArgs e)
        {
            if (CurrentVideo != null)
            {
                VideoManager.GetInstance().AddVideo(CurrentVideo);

                //var a = CurrentVideo.ImgUrl.Split(new char[] { '/' });
                //ImageHelper.DoGetImage(CurrentVideo.ImgUrl, string.Format("{0}{1}\\{2}", Config.VIDEO_IMG_PATH, CurrentVideo.Code, a[a.Length - 1]));
                ImageHelper.DoGetImage(CurrentVideo.ImgUrl, string.Format("{0}{1}\\{2}", Config.VIDEO_IMG_PATH, CurrentVideo.Code, CurrentVideo.Code + "l.jpg"));

                //a = CurrentVideo.SubImgUrl.Split(new char[] { '/' });
                ImageHelper.DoGetImage(CurrentVideo.SubImgUrl, string.Format("{0}{1}\\{2}", Config.VIDEO_IMG_PATH, CurrentVideo.Code, CurrentVideo.Code + "s.jpg"));

                MessageBox.Show("保存成功");
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
