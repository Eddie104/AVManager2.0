using avManager.model.data;
using AVManager2.avManager;
using libra.db.mongoDB;
using Libra.helper;
using Libra.log4CSharp;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace avManager.model
{
    class VideoManager
    {
        private static VideoManager instance = null;

        private readonly string collectionName;

        private List<Video> videoList = new List<Video>();

        private VideoManager()
        {
            collectionName = "video";
        }

        public void Init(Action callback)
        {
            MongoCursor<BsonDocument> list = MongoDBHelper.Search(collectionName);
            Video v = null;
            if (list != null)
            {
                foreach (BsonDocument doc in list)
                {
                    v = new Video(doc);
                    this.videoList.Add(v);
                }
            }

            foreach(string path in Config.VIDEO_PATH)
            {
                CheckVideo(path);
            }
            
            //// 将重复的classType删除
            //Dictionary<string, List<ClassType>> repeatClassType = ClassTypeManager.GetInstance().GetRepeatClassType();
            //// 遍历所有video
            //foreach (var video in videoList)
            //{
            //    // 遍历所有重复的classType
            //    foreach (var keyVal in repeatClassType)
            //    {
            //        foreach (var classType in keyVal.Value)
            //        {
            //            // 一旦发现video中包含了重复的classType
            //            // 就把video中重复的classType都删掉，然后加入repeatClassType中的第一个classType
            //            if (video.ClassList.Contains(classType.ID))
            //            {
            //                int index = 0;
            //                foreach (var classType1 in keyVal.Value)
            //                {
            //                    if (index++ > 0)
            //                    {
            //                        classType1.NeedDelete = true;
            //                    }
            //                    video.ClassList.Remove(classType1.ID);
            //                }
            //                video.ClassList.Add(keyVal.Value[0].ID);
            //                break;
            //            }
            //        }
            //    }
            //}
            callback();
        }

        private void CheckVideo(string path)
        {
            //看看目录下有哪些是数据库里没有的影片，也都加进来
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            Video v = null;
            foreach (DirectoryInfo d in dirInfo.GetDirectories())
            {
                v = GetVideo(d.Name);
                //if (File.Exists(d.FullName + "\\" + d.Name + ".json"))
                //{

                //}
                if (v == null)
                {
                    v = new Video(new ObjectId(ObjectIdGenerator.Generate()), d.Name);
                    AddVideo(v);
                }
                if(v.Path != path)
                {
                    Logger.Error(string.Format("发现相同番号;{0}", v.Code));
                }
                v.Path = path;
            }
        }

        public Video AddVideo(Video v)
        {
            if (v != null)
            {
                if (GetVideo(v.Code) == null)
                {
                    this.videoList.Add(v);
                    v.NeedInsert = true;
                }
            }
            return v;
        }

        public Video RemoveVideo(ObjectId id)
        {
            foreach (Video v in videoList)
            {
                if (v.ID == id)
                {
                    v.NeedDelete = true;
                    return v;
                }
            }
            return null;
        }

        public void UpdateVideo(ObjectId id, Dictionary<string, object> property)
        {
            Video v = this.GetVideo(id);
            if (v != null)
            {
                v.Update(property);
                v.NeedUpdate = true;
            }
        }

        public Video GetVideo(ObjectId id)
        {
            foreach (Video v in videoList)
            {
                if (v.ID == id)
                {
                    return v;
                }
            }
            return null;
        }

        public Video GetVideo(string code)
        {
            foreach (Video v in videoList)
            {
                if (v.Code == code)
                {
                    return v;
                }
            }
            return null;
        }

        public List<Video> GetVideoList(ObjectId classTypeID, string code, SortType sortType, bool desc)
        {
            var list = GetVideoList(code);
            if(classTypeID != ObjectId.Empty)
            {
                list.RemoveAll(v => !v.ClassList.Contains(classTypeID));
            }
            if(sortType == SortType.VideoCode)
            {
                list.Sort(new SortByVideoCodeCamparer(desc));
            }else if(sortType == SortType.VideoBirthday)
            {
                list.Sort(new SortByVideoCodeBirthday(desc));
            }
            return list;
        }

        public List<Video> GetVideoList(ObjectId actressID)
        {
            List<Video> list = new List<Video>();
            foreach (Video v in videoList)
            {
                if (v.HasActress(actressID))
                {
                    list.Add(v);
                }
            }
            return list;
        }

        public List<Video> GetVideoList(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return new List<Video>(this.videoList.ToArray());
            }
            code = code.ToUpper();
            List<Video> list = new List<Video>();
            foreach (Video v in videoList)
            {
                if (v.Code.Contains(code))
                {
                    list.Add(v);
                }
            }
            return list;
        }

        public void SaveToDB(Action callback)
        {
            List<Video> tmp = new List<Video>(videoList.ToArray());
            foreach (Video v in tmp)
            {
                if (v.NeedDelete)
                {
                    if (MongoDBHelper.Remove(collectionName, new QueryDocument("_id", v.ID)))
                    {
                        videoList.Remove(v);
                    }
                }
                else if (v.NeedUpdate)
                {
                    if (MongoDBHelper.Update(collectionName, new QueryDocument("_id", v.ID), v.CreateMongoUpdate()))
                    {
                        v.NeedUpdate = false;
                    }
                }
                else if (v.NeedInsert)
                {
                    if (MongoDBHelper.Insert(collectionName, v.CreateBsonDocument()))
                    {
                        v.NeedInsert = false;
                    }
                }
            }
            callback();
        }

        /// <summary>
        /// 解析从javbus上获取到的html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="code"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Video CreateVideoFromJav(string html, string code, Video v)
        {
            // 判断是否有码
            v.HasMask = !html.Contains("ovie-box-uncensored.css");
            //<a class="bigImage" href="https://images.javbus.info/cover/d7k_b.jpg"><img
            v.ImgUrl = Regex.Match(html, "<a class=\"bigImage\".*_b.jpg\"><img").ToString().Replace("<a class=\"bigImage\" href=\"", "").Replace("\"><img", "");
            // <h3>LAF-41 ラフォーレ ガール Vol.41 天使と悪魔 : 大橋未久 </h3>
            v.Name = Regex.Match(html, "<h3>.*</h3>").ToString().Replace("<h3>", "").Replace("</h3>", "");
            //:#CC0000;">LAF-41</span>
            v.Code = Regex.Match(html, ":#CC0000;\">\\w+-\\d+</span>").ToString().Replace(":#CC0000;\">", "").Replace("</span>", "");
            v.Date = DateTime.Parse(Regex.Match(html, @"\d{4}-\d{2}-\d{2}").ToString());
            /*
            <span class="genre"><a href="https://www.javbus.me/genre/e">巨乳</a></span>
            */
            var tdList = Regex.Matches(html, "<span class=\"genre\"><a href=\"https://www.javbus.me/genre/[a-z0-9]+.*</a></span>");
            string item;
            for (int i = 0; i < tdList.Count; i++)
            {
                item = tdList[i].ToString();
                item = Strings.StrConv(item, VbStrConv.SimplifiedChinese);
                item = Regex.Replace(item.Replace("</a></span>", ""), "<span class=\"genre\"><a href=\"https://www.javbus.me/genre/[a-z0-9]+\">", "");
                ClassType classType = ClassTypeManager.GetInstance().GetClassType(item);
                if (!v.ClassList.Contains(classType.ID))
                    v.ClassList.Add(classType.ID);
            }
            //<a href="https://www.javbus.me/star/2yl">あやみ旬果</a>
            Actress a = null;
            //<a href="https://www.javbus.me/uncensored/star/78x">大橋未久</a>
            tdList = Regex.Matches(html, "<a href=\"https://www.javbus.me/(uncensored/)?star/[a-z0-9]+\">\\w+</a>");
            for (int i = 0; i < tdList.Count; i++)
            {
                item = tdList[i].ToString();
                item = Regex.Replace(item.Replace("</a>", ""), "<a href=\"https://www.javbus.me/(uncensored/)?star/[a-z0-9]+\">", "");
                a = ActressManager.GetInstance().GetActress(item, true);
                if (a != null)
                {
                    if (!v.ActressList.Contains(a.ID))
                        v.ActressList.Add(a.ID);
                }
            }
            return v;
        }

        public Video CreateVideo(string html, string code, Video v = null)
        {
            if (v == null)
            {
                v = new Video();
            }
            // 这边的video都是有码的
            v.HasMask = true;
            //http://pics.dmm.co.jp/mono/movie/adult/118sga033/118sga033ps.jpg
            //http://pics.dmm.co.jp/mono/movie/adult/118sga033/118sga033pl.jpg
            v.ImgUrl = Regex.Match(Regex.Match(html, "<img id=\\\"video_jacket_img\\\" src=\\\".* width=").ToString(), "http.*jpg").ToString();

            v.Name = Regex.Match(Regex.Match(html, "<div id=\"video_title.*</a></h3>").ToString(), @"[A-Z]+-\d+\s.*</a>").ToString().Replace("</a>", "");

            var tdList = Regex.Matches(html, @"<td.+?>(?<content>.+?)</td>");
            string item;
            //提取链接的内容
            Regex regA = new Regex(@"<a\s+href=(?<url>.+?)>(?<content>.+?)</a>");
            //[url=http://www.yimuhe.com/file-2813234.html][b]SHKD-638种子下载[/b][/url]
            Regex regUrl = new Regex(@"\[url=.*/url\]");
            for (int i = 0; i < tdList.Count; i++)
            {
                item = tdList[i].ToString();
                if (item.Contains("识别码:"))
                {
                    v.Code = tdList[i + 1].ToString().Replace("<td class=\"text\">", "").Replace("</td>", "");
                }
                else if (item.Contains("发行日期:"))
                {
                    v.Date = DateTime.Parse(tdList[i + 1].ToString().Replace("<td class=\"text\">", "").Replace("</td>", ""));
                }
                else if (item.Contains("类别:"))
                {
                    Regex regPlace = new Regex("<a href=\"vl_genre.php\\?g=\\w+\" rel=\"category tag\">");
                    var classList = regA.Matches(tdList[i + 1].ToString());
                    foreach (var classItem in classList)
                    {
                        ClassType classType = ClassTypeManager.GetInstance().GetClassType(regPlace.Replace(classItem.ToString(), "").Replace("</a>", ""));
                        if(!v.ClassList.Contains(classType.ID))
                            v.ClassList.Add(classType.ID);
                    }
                }
                else if (item.Contains("演员:"))
                {
                    Actress a;
                    Regex regPlace = new Regex("<a href=\"vl_star.php\\?s=\\w+\" rel=\"tag\">");
                    //{<td class="text"><span id="cast3564" class="cast"><span class="star"><a href="vl_star.php?s=o45a" rel="tag">川上优</a></span> <span id="alias3680" class="alias">森野雫</span> <span id="star_o45a" class="icn_favstar" title="将这演员加入我最爱的演员名单。"></span></span></td>}

                    var classList = regA.Matches(tdList[i + 1].ToString());
                    string actressName = null;
                    foreach (var classItem in classList)
                    {
                        actressName = regPlace.Replace(classItem.ToString(), "").Replace("</a>", "");
                        a = ActressManager.GetInstance().GetActress(actressName, true);
                        if (a != null)
                        {
                            if(!v.ActressList.Contains(a.ID))
                                v.ActressList.Add(a.ID);
                        }
                        //else
                        //{
                        //    a = ActressManager.GetInstance().AddActress(actressName, "", new DateTime(), 0, 0, 0, 0, "X");
                        //    v.ActressList.Add(a.ID);
                        //}
                    }
                    var regAlias = new Regex("class=\"alias\">\\w+</span>");
                    classList = regAlias.Matches(tdList[i + 1].ToString());
                    foreach (var classItem in classList)
                    {
                        a = ActressManager.GetInstance().GetActress(classItem.ToString().Replace("</span>", "").Replace("class=\"alias\">", ""), true);
                        if (a != null)
                        {
                            if (!v.ActressList.Contains(a.ID))
                                v.ActressList.Add(a.ID);
                        }
                    }
                }
                else if (item.Contains("[url="))
                {
                    item = regUrl.Match(item).ToString();
                    v.TorrentList.Add(item.Split(new char[] { ']' })[0].Replace("[url=", ""));
                }
            }
            return v;
        }

        public static VideoManager GetInstance()
        {
            if (instance == null)
            {
                instance = new VideoManager();
            }
            return instance;
        }
    }

    class SortByVideoCodeCamparer : IComparer<Video>
    {

        public bool Desc { get; set; }

        public SortByVideoCodeCamparer(bool desc)
        {
            Desc = desc;
        }

        public int Compare(Video a, Video b)
        {
            return Desc ? b.Code.CompareTo(a.Code) : a.Code.CompareTo(b.Code);
        }
    }

    class SortByVideoCodeBirthday : IComparer<Video>
    {

        public bool Desc { get; set; }

        public SortByVideoCodeBirthday(bool desc)
        {
            Desc = desc;
        }

        public int Compare(Video a, Video b)
        {
            int result = b.Date.CompareTo(a.Date);
            if (result == 0)
            {
                return Desc ? b.Code.CompareTo(a.Code) : a.Code.CompareTo(b.Code);
            }
            else
            {
                return Desc ? result : -result;
            }
        }
    }

    enum SortType {
        VideoCode,
        VideoBirthday
    }
}
