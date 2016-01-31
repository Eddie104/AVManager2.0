using avManager.model;
using avManager.model.data;
using AVManager2.avManager.view.actress;
using libra.web;
using Libra.helper;
using Libra.log4CSharp;
using MahApps.Metro.Controls;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AVManager2.avManager.view.video
{
    /// <summary>
    /// VideoDetail.xaml 的交互逻辑
    /// </summary>
    public partial class VideoDetail : MetroWindow
    {

        public event EventHandler UpdateHandler;

        public VideoDetail()
        {
            InitializeComponent();
        }

        private Video video;
        public Video Video
        {
            get { return video; }
            set
            {
                video = value;
                string imgPath = string.Format("{0}{1}\\{2}l.jpg", Config.VIDEO_PATH, video.Code, video.Code);
                if (File.Exists(imgPath))
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(imgPath));
                    img.Source = bitmap;
                    img.Width = bitmap.PixelWidth;
                    img.Height = bitmap.PixelHeight;
                }
                else
                {
                    Logger.Error(string.Format("影片图片:{0}不存在", imgPath));
                }
                this.codeLabel.Content = video.Code;
                this.dateLabel.Content = video.Date.ToShortDateString();
                this.classLabel.Content = video.GetClassString();
                //this.actressLabel.Content = video.GetActressString();
                this.nameLabel.Content = video.Name;

                ActressInfo info = null;
                Actress actress;
                foreach(var id in video.ActressList)
                {
                    actress = ActressManager.GetInstance().GetActress(id);
                    info = new ActressInfo();
                    info.Actress = actress;
                    info.Click += OnActressInfoClicked;
                    this.actressContainer.Children.Add(info);
                }
            }
        }

        private void OnActressInfoClicked(object sender, EventArgs e)
        {
            ActressDetail ad = new ActressDetail();
            ad.Actress = (sender as ActressInfo).Actress;
            ad.ShowDialog();
        }

        private void OnUpdateVideo(object sender, RoutedEventArgs e)
        {
            HTMLHelper.GetInstance().GetHtml(string.Format("http://www.javlibrary.com/cn/vl_searchbyid.php?keyword={0}", video.Code), this.callback);
        }

        private void callback(string html)
        {
            if (html.Contains("识别码搜寻结果"))
            {
                //<div class="video" id="vid_javlio354y"><a href="./?v=javlio354y" title="ABP-001 水咲ローラがご奉仕しちゃう超最新やみつきエステ"><div class="id">ABP-001</div>
                //http://www.javlibrary.com/cn/?v=javlio354y
                Regex regVideo = new Regex("<div class=\"video\".*" + video.Code + "</div>");
                var videoItem = regVideo.Match(html);
                if (!string.IsNullOrEmpty(videoItem.Value))
                {                    
                    var m = new Regex("<a href=.* title=").Match(videoItem.Value);
                    var t = m.Value.Replace("\"", "").Replace("<a href=./", "").Replace(" title=", "");
                    HTMLHelper.GetInstance().GetHtml(string.Format("http://www.javlibrary.com/cn/{0}", t), this.callback);
                }
                else
                {
                    //javlibrary找不到影片信息上找不到，那就到javmoo上去找
                    //HTMLHelper.GetInstance().GetHtml(string.Format("http://www.javmoo.xyz/cn/search/{0}", video.Code), this.callbackOnJav);
                    MessageBox.Show("找不到相关信息");
                }
            }
            else
            {
                VideoManager.GetInstance().CreateVideo(html, Video.Code, Video);
                this.onCreateVideo();
            }
        }

        private void callbackOnJav(string html)
        {
            if (html.Contains("搜寻没有结果"))
            {
                MessageBox.Show("搜寻没有结果");
            }
            else
            {
                //v.ImgUrl = Regex.Match(Regex.Match(html, "<img id=\\\"video_jacket_img\\\" src=\\\".* width=").ToString(), "http.*jpg").ToString();
                string s = Regex.Match(html, "<div class=\"item\">.*" + video.Code + "</date>").ToString();
                MessageBox.Show(s);

                //VideoManager.GetInstance().CreateVideoFromJav(html, Video.Code, Video);
                //this.onCreateVideo();
            }
        }

        private void onCreateVideo()
        {
            Video.NeedUpdate = true;

            string path = string.Format("{0}{1}\\{2}", Config.VIDEO_PATH, Video.Code, Video.Code + "l.jpg");
            if (!File.Exists(path))
            {
                ImageHelper.DoGetImage(Video.ImgUrl, path);
            }
            path = string.Format("{0}{1}\\{2}", Config.VIDEO_PATH, Video.Code, Video.Code + "s.jpg");
            if (!File.Exists(path))
            {
                ImageHelper.DoGetImage(Video.SubImgUrl, path);
            }
            //DialogManager.ShowMessageAsync(this, "更新完成", "更新完成");
            MessageBox.Show("ok");
            //Video = Video;
            //UpdateHandler(Video, null);
            //this.Dispatcher.Invoke(new Action(() ={ this.UpdateHandler(Video, null); }));
            this.Dispatcher.Invoke(new Action(() => { UpdateHandler(Video, null); Video = Video; }));
        }

        //private void OnCopyActress(object sender, RoutedEventArgs e)
        //{
        //    Clipboard.SetText(Video.GetActressString());
        //}
    }
}
