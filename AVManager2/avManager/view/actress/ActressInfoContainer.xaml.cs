using avManager.model;
using avManager.model.data;
using Libra.helper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AVManager2.avManager.view.actress
{
    /// <summary>
    /// ActressInfoContainer.xaml 的交互逻辑
    /// </summary>
    public partial class ActressInfoContainer : UserControl
    {

        private ActressManager actressManager;

        private const int TOTAL_NUM_PER_PAGE = 44;

        private int curPage = 1;

        private int totalPage = 1;

        private List<ActressInfo> actressInfoList = new List<ActressInfo>();

        private List<Actress> actressList;

        public ActressInfoContainer()
        {
            InitializeComponent();

            ActressInfo info = null;
            for (int i = 0; i < TOTAL_NUM_PER_PAGE; i++)
            {
                info = new ActressInfo();
                this.actressInfoContainer.Children.Add(info);
                actressInfoList.Add(info);
                info.Click += OnActressInfoClicked;
            }
        }        

        public void InitActressInfo()
        {
            actressManager = ActressManager.GetInstance();
            FilterActress(new ActressFilter() { SortByScoreDesc = true });
        }

        private void OnActressInfoClicked(object sender, EventArgs e)
        {
            //Console.WriteLine("a = {0}", (sender as ActressInfo).Actress.Name);
            ActressDetail ad = new ActressDetail();
            ad.Actress = (sender as ActressInfo).Actress;
            ad.ShowDialog();
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

        private void PageChanged(int newPage, bool force = false)
        {
            newPage = Math.Max(1, Math.Min(totalPage, newPage));
            if (newPage != curPage || force)
            {
                curPage = newPage;
                pageLabel.Text = string.Format("{0}/{1}", curPage, totalPage);
                this.ShowActressInfo();
            }
        }

        private void ShowActressInfo()
        {
            int startIndex = (curPage - 1) * TOTAL_NUM_PER_PAGE;
            int endIndex = Math.Min(actressList.Count - 1, startIndex + TOTAL_NUM_PER_PAGE - 1);
            int index = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                this.actressInfoList[index++].Actress = actressList[i];
            }
            for (int i = index; i < TOTAL_NUM_PER_PAGE; i++)
            {
                this.actressInfoList[i].Actress = null;
            }
        }

        private void OnTextPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!StringHelper.isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !StringHelper.isNumberic(e.Text);
        }

        private void OnFilterActressInfo(object sender, TextChangedEventArgs e)
        {
            if (this.IsInitialized)
            {
                ActressFilter af = new ActressFilter();
                af.NameKeyWord = this.nameKeyWordTextBox.Text;
                int minHeight = 0, maxHeight = 0;
                int.TryParse(this.minHeightTextBox.Text, out minHeight);
                int.TryParse(this.maxHeightTextBox.Text, out maxHeight);
                af.MinHeight = minHeight;
                af.MaxHeight = maxHeight;
                af.SortByScoreDesc = sortByScoreComboBox.SelectedIndex == 0;

                this.FilterActress(af);
            }
        }

        private void FilterActress(ActressFilter filter)
        {
            this.actressList = actressManager.GetActressList(filter.NameKeyWord, filter.MinHeight, filter.MaxHeight, filter.SortByScoreDesc);
            totalPage = (int)(Math.Ceiling((double)actressList.Count / TOTAL_NUM_PER_PAGE));
            PageChanged(1, true);
        }

        struct ActressFilter
        {
            public string NameKeyWord { get; set; }

            public int MinHeight { get; set; }

            public int MaxHeight { get; set; }

            public bool SortByScoreDesc { get; set; }
        }

        private void OnShowCreateActress(object sender, RoutedEventArgs e)
        {
            CreateActress win = new CreateActress();
            win.ShowDialog();
        }
    }
}
