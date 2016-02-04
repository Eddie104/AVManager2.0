using avManager.model;
using avManager.model.data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AVManager2.avManager.view.video
{
    /// <summary>
    /// VideoInfoContainer.xaml 的交互逻辑
    /// </summary>
    public partial class VideoInfoContainer : Grid
    {

        private const int TOTAL_NUM_PER_PAGE = 27;

        private VideoManager videoManager;

        private List<Video> videoList;

        private int curPage = 1;

        private int totalPage = 1;

        private List<VideoInfo> videoInfoList = new List<VideoInfo>();

        public VideoInfoContainer()
        {
            InitializeComponent();

            VideoInfo info = null;
            for (int i = 0; i < TOTAL_NUM_PER_PAGE; i++)
            {
                info = new VideoInfo();
                this.videoInfoContainer.Children.Add(info);
                videoInfoList.Add(info);
                info.Click += OnVideoInfoClicked;
            }
        }

        private void OnVideoInfoClicked(object sender, EventArgs e)
        {
            VideoDetail ad = new VideoDetail();
            ad.UpdateHandler += OnUpdateHandler;
            ad.Video = (sender as VideoInfo).Video;
            ad.ShowDialog();
        }

        private void OnUpdateHandler(object sender, EventArgs e)
        {
            Video v = sender as Video;
            foreach (var item in videoInfoList)
            {
                if (item.UpdateVideo(v))
                {
                    break;
                }
            }
        }

        public void InitVideoInfo()
        {
            videoManager = VideoManager.GetInstance();
            FilterVideo();

            this.classTypeComboBox.Items.Add("所有");
            foreach (var item in ClassTypeManager.GetInstance().GetClassType())
            {
                this.classTypeComboBox.Items.Add(item);
            }
        }

        private void PageChanged(int newPage, bool force = false)
        {
            newPage = Math.Max(1, Math.Min(totalPage, newPage));
            if (newPage != curPage || force)
            {
                curPage = newPage;
                pageLabel.Content = string.Format("{0}/{1}", curPage, totalPage);
                this.ShowVideoInfo();
            }
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrevPageHander(object sender, RoutedEventArgs e)
        {
            PageChanged(Math.Max(1, curPage - 1));
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextPageHandler(object sender, RoutedEventArgs e)
        {
            PageChanged(Math.Min(totalPage, curPage + 1));
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

        private void OnShowNewVideoWindow(object sender, RoutedEventArgs e)
        {
            NewVideoWindow newVideoWindow = new NewVideoWindow();
            newVideoWindow.ShowDialog();
        }

        private void onSortChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsInitialized)
            {
                FilterVideo();
            }
        }

        /// <summary>
        /// 类别筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClassTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterVideo();
        }

        private void FilterVideo()
        {
            SortType st = SortType.VideoBirthday;
            if (sortTypeComboBox.SelectedIndex == 1)
            {
                st = SortType.VideoCode;
            }

            ClassType selectItem = classTypeComboBox.SelectedIndex == 0 ? null : classTypeComboBox.SelectedItem as ClassType;

            this.videoList = videoManager.GetVideoList(selectItem != null ? selectItem.ID : ObjectId.Empty, st, false);
            totalPage = (int)(Math.Ceiling((double)videoList.Count / TOTAL_NUM_PER_PAGE));
            PageChanged(1, true);
        }
    }
}
