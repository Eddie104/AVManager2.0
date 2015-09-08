using System;
using System.Windows;
using AVManager2.gg;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using AVManager2.libra.web;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
            //if(test.Modify())
            //{
            //    Console.WriteLine("修改成功");
            //}
            //else
            //{
            //    Console.WriteLine("修改失败");
            //}
            //HTMLHelper.GetInstance().GetHtml("http://www.javlibrary.com/cn/vl_searchbyid.php?keyword=SHKD-638", this.callback);

            string s = "";
            using (StreamReader sr = new StreamReader("tt.txt", Encoding.Default))
            {
                s = sr.ReadToEnd();
            }
            this.ttt(s);
        }

        private void callback(string html)
        {
            Console.WriteLine(html);
        }

        private void ttt(string html)
        {
            string imgURL = Regex.Match(Regex.Match(html, "<img id=\\\"video_jacket_img\\\" src=\\\".* width=").ToString(), "http.*jpg").ToString();
            Console.WriteLine("图片名 = {0}", imgURL);

            string name = Regex.Match(Regex.Match(html, "<div id=\"video_title.*</a></h3>").ToString(), @"[A-Z]+-\d+\s.*</a>").ToString().Replace("</a>", "");
            Console.WriteLine("片名 = {0}", name);

            var tdList = Regex.Matches(html, @"<td.+?>(?<content>.+?)</td>");
            string item;
            //提取链接的内容
            Regex regA = new Regex(@"<a\s+href=(?<url>.+?)>(?<content>.+?)</a>");
            //[url=http://www.yimuhe.com/file-2813234.html][b]SHKD-638种子下载[/b][/url]
            Regex regUrl = new Regex(@"\[url=.*/url\]");
            for (int i = 0; i < tdList.Count; i++)
            {
                item = tdList[i].ToString();
                if (item.Contains("识别码"))
                {
                    Console.WriteLine("识别码 = {0}", tdList[i + 1].ToString().Replace("<td class=\"text\">", "").Replace("</td>", ""));
                }
                else if (item.Contains("发行日期"))
                {
                    Console.WriteLine("发行日期 = {0}", tdList[i + 1].ToString().Replace("<td class=\"text\">", "").Replace("</td>", ""));
                }
                else if (item.Contains("类别"))
                {
                    Regex regPlace = new Regex("<a href=\"vl_genre.php\\?g=\\w+\" rel=\"category tag\">");
                    var classList = regA.Matches(tdList[i + 1].ToString());
                    foreach (var classItem in classList)
                    {
                        Console.WriteLine("类别 = {0}", regPlace.Replace(classItem.ToString(), "").Replace("</a>", ""));
                    }
                }
                else if (item.Contains("演员"))
                {
                    Regex regPlace = new Regex("<a href=\"vl_star.php\\?s=\\w+\" rel=\"tag\">");
                    var classList = regA.Matches(tdList[i + 1].ToString());
                    foreach (var classItem in classList)
                    {
                        Console.WriteLine("演员 = {0}", regPlace.Replace(classItem.ToString(), "").Replace("</a>", ""));
                    }
                }
                else if (item.Contains("[url="))
                {
                    item = regUrl.Match(item).ToString();
                    Console.WriteLine("种子下载地址 = {0}", item.Split(new char[] { ']' })[0].Replace("[url=", ""));
                }
            }
        }
    }
}
