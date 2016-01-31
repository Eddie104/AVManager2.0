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

        private string code;

        public NewVideoWindow()
        {
            InitializeComponent();
        }

        private void OnSearchVideoInfo(object sender, RoutedEventArgs e)
        {
            code = this.codeTextBox.Text.ToUpper();
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
            if (html.Contains("识别码搜寻结果"))
            {
                //<div class="video" id="vid_javlio354y"><a href="./?v=javlio354y" title="ABP-001 水咲ローラがご奉仕しちゃう超最新やみつきエステ"><div class="id">ABP-001</div>
                //http://www.javlibrary.com/cn/?v=javlio354y
                Regex regVideo = new Regex("<div class=\"video\".*" + code + "</div>");
                var videoItem = regVideo.Match(html);
                if (!string.IsNullOrEmpty(videoItem.Value))
                {
                    var m = new Regex("<a href=.* title=").Match(videoItem.Value);
                    var t = m.Value.Replace("\"", "").Replace("<a href=./", "").Replace(" title=", "");
                    HTMLHelper.GetInstance().GetHtml(string.Format("http://www.javlibrary.com/cn/{0}", t), this.callback);
                }
                else
                {
                    MessageBox.Show("找不到影片信息:" + code);
                }
            }
            else
            {
                CurrentVideo = VideoManager.GetInstance().CreateVideo(html, code);

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
        }

        private void OnAddHandler(object sender, RoutedEventArgs e)
        {
            if (CurrentVideo != null)
            {
                VideoManager.GetInstance().AddVideo(CurrentVideo);
                ImageHelper.DoGetImage(CurrentVideo.ImgUrl, string.Format("{0}{1}\\{2}", Config.VIDEO_PATH, CurrentVideo.Code, CurrentVideo.Code + "l.jpg"));
                ImageHelper.DoGetImage(CurrentVideo.SubImgUrl, string.Format("{0}{1}\\{2}", Config.VIDEO_PATH, CurrentVideo.Code, CurrentVideo.Code + "s.jpg"));
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
