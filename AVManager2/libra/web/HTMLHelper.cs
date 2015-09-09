using libra.util;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;

namespace libra.web
{
    class HTMLHelper
    {

        private class RequestState
        {
            
            public HttpWebRequest Req { get; private set; }
            public string Url { get; private set; }
            public Stream ResStream { get; set; }

            private StringBuilder _sb = new StringBuilder();
            public StringBuilder Html
            {
                get { return _sb; }
            }

            private const int BUFFER_SIZE = 131072;
            public int BufferSize
            {
                get { return BUFFER_SIZE; }
            }

            private byte[] _data = new byte[BUFFER_SIZE];
            public byte[] Data
            {
                get {return _data;}
            }

            public RequestState(HttpWebRequest req, string url)
            {
                Req = req;
                Url = url;
            }
        }

        private static HTMLHelper instance;

        private Action<string> callback;

        private HTMLHelper()
        {

        }

        public void GetHtml(string url, Action<string> callback, Encoding encoding = null)
        {
            if(RegularHelper.IsUrl(url))
            {
                this.callback = callback;
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }

                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                //请求方法
                req.Method = "GET";
                //接受的内容
                req.Accept = "text/html";
                //用户代理
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)";
                RequestState rs = new RequestState(req, url);
                var result = req.BeginGetResponse(new AsyncCallback(ReceivedResource), rs);
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, TimeoutCallback, rs, 120000, true);
            }
            else
            {
                MessageBox.Show(string.Format("错误的url:{0}", url));
            }
        }

        private void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                RequestState rs = state as RequestState;
                if (rs != null)
                {
                    rs.Req.Abort();
                }
                MessageBox.Show(string.Format("访问{0}时超时了，请稍后再试", rs.Url));
            }
        }

        private void ReceivedResource(IAsyncResult ar)
        {
            RequestState rs = (RequestState)ar.AsyncState;
            HttpWebRequest req = rs.Req;
            string url = rs.Url;
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.EndGetResponse(ar);
                if (res != null && res.StatusCode == HttpStatusCode.OK)
                {
                    Stream resStream = res.GetResponseStream();
                    rs.ResStream = resStream;
                    var result = resStream.BeginRead(rs.Data, 0, rs.BufferSize,
                        new AsyncCallback(ReceivedData), rs);
                }
                else
                {
                    res.Close();
                    rs.Req.Abort();
                }
            }
            catch (WebException we)
            {
                MessageBox.Show("ReceivedResource " + we.Message + url + we.Status);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ReceivedData(IAsyncResult ar)
        {
            RequestState rs = (RequestState)ar.AsyncState;
            HttpWebRequest req = rs.Req;
            Stream resStream = rs.ResStream;
            string url = rs.Url;
            string html = null;
            int read = 0;

            try
            {
                read = resStream.EndRead(ar);
                if (read > 0)
                {
                    MemoryStream ms = new MemoryStream(rs.Data, 0, read);
                    StreamReader reader = new StreamReader(ms, Encoding.UTF8);
                    string str = reader.ReadToEnd();
                    rs.Html.Append(str);
                    var result = resStream.BeginRead(rs.Data, 0, rs.BufferSize, new AsyncCallback(ReceivedData), rs);
                }
                else
                {
                    html = rs.Html.ToString();
                    this.callback(html);
                }
            }
            catch (WebException we)
            {
                MessageBox.Show("ReceivedData Web " + we.Message + url + we.Status);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetType().ToString() + e.Message);
            }
        }

        public static HTMLHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new HTMLHelper();
            }
            return instance;
        }

    }
}
