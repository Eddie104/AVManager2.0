using avManager.model;
using libra.db.mongoDB;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace AVManager2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ClassTypeManager classTypeManager;

        private ActressManager actressManager;

        private VideoManager videoManager;

        public MainWindow()
        {
            InitializeComponent();

            MongoDBHelper.connectionString = "mongodb://localhost";
            MongoDBHelper.dbName = "avdb";

            classTypeManager = ClassTypeManager.GetInstance();
            classTypeManager.Init();

            actressManager = ActressManager.GetInstance();
            actressManager.Init();

            videoManager = VideoManager.GetInstance();
            videoManager.Init();

            actressInfoContainer.InitActressInfo();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            actressManager.AddActress("name123", "", DateTime.Now, 160, 90, 60, 90, "C");
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
            
        }
    }
}
