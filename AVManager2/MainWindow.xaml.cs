using System;
using System.Windows;
using AVManager2.gg;

namespace AVManager2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MongoDBTest test;

        public MainWindow()
        {
            InitializeComponent();

            test = new MongoDBTest();

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            if (test.Insert())
            {
                Console.WriteLine("ok");
            }
            else
            {
                Console.WriteLine("no ok");
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            test.Select();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(test.Modify())
            {
                Console.WriteLine("修改成功");
            }
            else
            {
                Console.WriteLine("修改失败");
            }
        }
    }
}
