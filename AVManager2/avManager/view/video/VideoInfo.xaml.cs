using avManager.model.data;
using Libra.log4CSharp;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AVManager2.avManager.view.video
{
    /// <summary>
    /// VideoInfo.xaml 的交互逻辑
    /// </summary>
    public partial class VideoInfo : UserControl
    {
        public VideoInfo()
        {
            InitializeComponent();
        }

        public event EventHandler Click;

        private Video video;

        public Video Video
        {
            get { return video; }
            set
            {
                video = value;
                if (video == null)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;

                    string imgPath = string.Format("{0}{1}\\{2}s.jpg", video.Path, video.Code, video.Code);
                    if (File.Exists(imgPath))
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(imgPath));
                        subCoverImg.Source = bitmap;
                        subCoverImg.Width = bitmap.PixelWidth;
                        subCoverImg.Height = bitmap.PixelHeight;
                    }
                    else
                    {
                        Logger.Error(string.Format("影片图片:{0}不存在", imgPath));
                        subCoverImg.Source = null;
                    }
                    nameLabel.Content = video.Name;
                    codeLabel.Content = video.Code;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
            e.Handled = false;
        }

        internal bool UpdateVideo(Video v)
        {
            if (Video.Code == v.Code)
            {
                Video = v;
                return true;
            }
            return false;
        }
    }
}
