using System.Collections.Generic;
using System.IO;
using System.Net;

namespace libra.util
{
    class ImageHelper
    {
        public static void DoGetImage(string url, string path)
        {
            if (!File.Exists(path))
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.ServicePoint.Expect100Continue = false;
                req.Method = "GET";
                req.KeepAlive = true;

                req.ContentType = "image/jpg";
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                Stream stream = null;
                try
                {
                    // 以字符流的方式读取HTTP响应
                    stream = rsp.GetResponseStream();

                    List<string> list = new List<string>(path.Split(new char[] { '\\' }));
                    list.RemoveAt(list.Count - 1);
                    string dir = "";
                    foreach (string s in list)
                    {
                        dir += s + "\\";
                    }
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    System.Drawing.Image.FromStream(stream).Save(path);
                }
                finally
                {
                    // 释放资源
                    if (stream != null) stream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
        }
    }
}
