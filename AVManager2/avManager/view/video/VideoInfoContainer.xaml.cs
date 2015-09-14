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

        private int curPage = 1;

        private int totalPage = 1;

        private List<VideoInfo> actressInfoList = new List<VideoInfo>();

        public VideoInfoContainer()
        {
            InitializeComponent();

            VideoInfo info = null;
            for (int i = 0; i < TOTAL_NUM_PER_PAGE; i++)
            {
                info = new VideoInfo();
                this.videoInfoContainer.Children.Add(info);
                actressInfoList.Add(info);
                //info.Click += OnActressInfoClicked;
            }
        }

        private void OnShowNewVideoWindow(object sender, RoutedEventArgs e)
        {
            NewVideoWindow newVideoWindow = new NewVideoWindow();
            newVideoWindow.ShowDialog();
        }
    }
}
