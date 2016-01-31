using avManager.model;
using libra.db.mongoDB;
using libra.web;
using Libra.helper;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace AVManager2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ClassTypeManager classTypeManager;

        private ActressManager actressManager;

        private VideoManager videoManager;

        public MainWindow()
        {
            InitializeComponent();

            MongoDBHelper.connectionString = "mongodb://localhost";
            MongoDBHelper.dbName = "avdb";

            Console.WriteLine("开始初始化");
            Console.WriteLine("开始初始化ClassType");
            classTypeManager = ClassTypeManager.GetInstance();
            classTypeManager.Init();

            Console.WriteLine("开始初始化Actress");
            actressManager = ActressManager.GetInstance();
            actressManager.Init();

            Console.WriteLine("开始初始化Video");
            videoManager = VideoManager.GetInstance();
            videoManager.Init();

            actressInfoContainer.InitActressInfo();
            videoInfoContainer.InitVideoInfo();
        }

        private int t = 11;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            actressManager.AddActress(string.Format("name{0}", t++), "", DateTime.Now, 160, 90, 60, 90, "C", "aa");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //HTMLHelper.GetInstance().GetHtml("http://www.javlibrary.com/cn/vl_searchbyid.php?keyword=SHKD-638", this.callback);

            //string s = "";
            //using (StreamReader sr = new StreamReader("tt.txt", Encoding.Default))
            //{
            //    s = sr.ReadToEnd();
            //}
            //videoManager.AddVideo(videoManager.CreateVideo(s));

            //actressManager.UpdateActress(actressManager.GetFirstActress().ID, new Dictionary<string, object>() {
            //    {"Name", "testName" }
            //});

        }

        //private void callback(string html)
        //{
        //    Console.WriteLine(html);
        //}



        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //test.Remove();
            //actressManager.RemoveActress(actressManager.GetFirstActress().ID);

            //actressManager.GetFirstActress().CreateBsonDocument();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            this.actressManager.SaveToDB();
            this.videoManager.SaveToDB();
            this.classTypeManager.SaveToDB();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            //ImageHelper.DoGetImage("http://n.sinaimg.cn/transform/20150914/-ybr-fxhupir7102739.jpg", "ttttt.jpg");
            HTMLHelper.GetInstance().GetHtml("http://www.javmoo.xyz/cn/search/AIR-004", this.callbackOnJav);
        }

        private void callbackOnJav(string html)
        {
            Console.WriteLine(html);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 退出之前，把数据库连接给关了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MongoDBHelper.Exit();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("This is the title", "Some message");
            DialogManager.ShowMessageAsync(this, "This is the title", "Some message");
        }
    }
}
