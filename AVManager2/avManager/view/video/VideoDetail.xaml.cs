using avManager.model;
using avManager.model.data;
using libra.web;
using Libra.helper;
using Libra.log4CSharp;
using MahApps.Metro.Controls;
using System;
using System.IO;
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
            }
        }

        private void OnUpdateVideo(object sender, RoutedEventArgs e)
        {
            HTMLHelper.GetInstance().GetHtml(string.Format("http://www.javlibrary.com/cn/vl_searchbyid.php?keyword={0}", video.Code), this.callback);
        }

        private void callback(string html)
        {
            VideoManager.GetInstance().CreateVideo(html, Video);
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
    }
}
