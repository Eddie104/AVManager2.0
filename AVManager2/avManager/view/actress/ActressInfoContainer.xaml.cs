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

namespace AVManager2.avManager.view.actress
{
    /// <summary>
    /// ActressInfoContainer.xaml 的交互逻辑
    /// </summary>
    public partial class ActressInfoContainer : UserControl
    {

        private ActressManager actressManager;

        private const int TOTAL_NUM_PER_PAGE = 4;

        private int curPage = 1;

        private int totalPage = 1;

        private List<ActressInfo> actressInfoList = new List<ActressInfo>();

        public ActressInfoContainer()
        {
            InitializeComponent();

            ActressInfo info = null;
            for (int i = 0; i < TOTAL_NUM_PER_PAGE; i++)
            {
                info = new ActressInfo();
                this.actressInfoContainer.Children.Add(info);
                actressInfoList.Add(info);
            }
        }

        public void InitActressInfo()
        {
            actressManager = ActressManager.GetInstance();
            totalPage = (int)Math.Ceiling((double)actressManager.GetActressList().Count / TOTAL_NUM_PER_PAGE);
            pageLabel.Text = string.Format("{0}/{1}", curPage, totalPage);

            List<Actress> actressList = actressManager.GetActressList((this.curPage - 1) * TOTAL_NUM_PER_PAGE, TOTAL_NUM_PER_PAGE);
            for (int i = 0; i < actressList.Count; i++)
            {
                this.actressInfoList[i].Actress = actressList[i];
            }
        }
    }
}
