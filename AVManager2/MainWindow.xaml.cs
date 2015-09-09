using System;
using System.Windows;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using libra.web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using libra.db.mongoDB;
using avManager.model;
using libra.util;

namespace AVManager2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ActressManager actressManager;

        private VideoManager videoManager;

        public MainWindow()
        {
            InitializeComponent();

            MongoDBHelper.connectionString = "mongodb://localhost";
            MongoDBHelper.dbName = "avdb";

            actressManager = ActressManager.GetInstance();
            actressManager.Init();

            videoManager = VideoManager.GetInstance();
            videoManager.Init();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            actressManager.AddActress("name123");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //HTMLHelper.GetInstance().GetHtml("http://www.javlibrary.com/cn/vl_searchbyid.php?keyword=SHKD-638", this.callback);

            string s = "";
            using (StreamReader sr = new StreamReader("tt.txt", Encoding.Default))
            {
                s = sr.ReadToEnd();
            }
            videoManager.AddVideo(videoManager.CreateVideo(s));

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

            actressManager.GetFirstActress().CreateBsonDocument();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            this.actressManager.SaveToDB();
            this.videoManager.SaveToDB();
        }
    }
}
