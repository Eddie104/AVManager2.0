using avManager.model;
using avManager.model.data;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AVManager2.avManager.view.video
{
    /// <summary>
    /// VideoInfoContainer.xaml 的交互逻辑
    /// </summary>
    public partial class VideoInfoContainer : UserControl
    {

        private const int TOTAL_NUM_PER_PAGE = 36;

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
                //info.Click += OnActressInfoClicked;
            }
        }

        public void InitVideoInfo()
        {
            videoManager = VideoManager.GetInstance();
            //FilterActress(new ActressFilter() { SortByScoreDesc = true });

            this.videoList = videoManager.GetVideoList();
            totalPage = (int)(Math.Ceiling((double)videoList.Count / TOTAL_NUM_PER_PAGE));
            PageChanged(1, true);
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
    }
}
