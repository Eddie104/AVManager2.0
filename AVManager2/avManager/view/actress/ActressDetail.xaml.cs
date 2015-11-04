using avManager.model;
using avManager.model.data;
using AVManager2.avManager.view.video;
using Libra.log4CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AVManager2.avManager.view.actress
{
    /// <summary>
    /// ActressDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ActressDetail : Window
    {

        private const int TOTAL_NUM_PER_PAGE = 11;

        private int curPage = 1;

        private int totalPage = 1;

        private List<Video> videoList;

        private Actress actress;
        public Actress Actress {
            get { return actress; }
            set
            {
                actress = value;
                nameLabel.Text = actress.Name;
                aliasLabel.Text = actress.Alias;
                ageLabel.Text = string.Format("年龄:{0}", DateTime.Now.Year - actress.Birthday.Year);
                heightLabel.Text = string.Format("身高:{0}", actress.Height);
                waistLabel.Text = string.Format("腰围:{0}", actress.Waist);
                bustLabel.Text = string.Format("胸围:{0}", actress.Bust);
                hipLabel.Text = string.Format("臀围:{0}", actress.Hip);
                cupLabel.Text = string.Format("罩杯:{0}", actress.Cup);
                scoreComboBox.SelectedIndex = actress.Score;

                string imgPath = string.Format("{0}{1}.jpg", Config.ACTRESS_IMG_PATH, actress.Code);
                if (File.Exists(imgPath))
                {
                    headImg.Source = new BitmapImage(new Uri(imgPath));
                }
                else
                {
                    Logger.Error(string.Format("图片:{0}.jpg不存在", actress.Code));
                }


                //int l = videoList.Count;
                //for (int i = 0; i < l; i++)
                //{
                //    videoInfoList[i]
                //}

                videoList = VideoManager.GetInstance().GetVideoList(actress.ID);
                totalPage = (int)(Math.Ceiling((double)videoList.Count / TOTAL_NUM_PER_PAGE));
                PageChanged(1, true);
            }
        }

        private List<VideoInfo> videoInfoList = new List<VideoInfo>();

        public ActressDetail()
        {
            InitializeComponent();

            VideoInfo videoInfo;
            for (int i = 0; i < TOTAL_NUM_PER_PAGE; i++)
            {
                videoInfo = new VideoInfo();
                videoContainer.Children.Add(videoInfo);
                videoInfoList.Add(videoInfo);
            }
        }

        private void PageChanged(int newPage, bool force = false)
        {
            newPage = Math.Max(1, Math.Min(totalPage, newPage));
            if (newPage != curPage || force)
            {
                curPage = newPage;
                //pageLabel.Text = string.Format("{0}/{1}", curPage, totalPage);
                this.ShowVideoInfo();
            }
        }

        private void ShowVideoInfo()
        {
            int startIndex = (curPage - 1) * TOTAL_NUM_PER_PAGE;
            int endIndex = Math.Min(videoList.Count - 1, startIndex + TOTAL_NUM_PER_PAGE - 1);
            int index = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                this.videoInfoList[index++].Video = videoList[i];
            }
            for (int i = index; i < TOTAL_NUM_PER_PAGE; i++)
            {
                this.videoInfoList[i].Video = null;
            }
        }

        private void scoreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Actress.Score = (sender as ComboBox).SelectedIndex;
            this.Actress.NeedUpdate = true;
        }
    }
}
