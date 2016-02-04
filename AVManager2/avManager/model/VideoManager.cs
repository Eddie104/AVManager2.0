using avManager.model.data;
using AVManager2.avManager;
using libra.db.mongoDB;
using Libra.helper;
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
            if (list != null)
            {
                foreach (BsonDocument doc in list)
                    this.videoList.Add(new Video(doc));
            }

            //看看目录下有哪些是数据库里没有的影片，也都加进来
            DirectoryInfo dirInfo = new DirectoryInfo(Config.VIDEO_PATH);
            foreach (DirectoryInfo d in dirInfo.GetDirectories())
            {
                if (GetVideo(d.Name) == null)
                {
                    AddVideo(new Video(new ObjectId(ObjectIdGenerator.Generate()), d.Name));
                }
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
            //                    if(index++ > 0)
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

        public List<Video> GetVideoList(ObjectId classTypeID, SortType sortType = SortType.VideoBirthday, bool desc = false)
        {
            var list = new List<Video>(this.videoList.ToArray());
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

        public void SaveToDB()
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
        }

        //public Video CreateVideoFromJav(string html, string code, Video v = null)
        //{
        //    if (v == null)
        //    {
        //        v = new Video();
        //    }

        //}

        public Video CreateVideo(string html, string code, Video v = null)
        {
            if (v == null)
            {
                v = new Video();
            }
            //http://pics.dmm.co.jp/mono/movie/adult/118sga033/118sga033ps.jpg
            //http://pics.dmm.co.jp/mono/movie/adult/118sga033/118sga033pl.jpg
            v.ImgUrl = Regex.Match(Regex.Match(html, "<img id=\\\"video_jacket_img\\\" src=\\\".* width=").ToString(), "http.*jpg").ToString();

            string name = Regex.Match(Regex.Match(html, "<div id=\"video_title.*</a></h3>").ToString(), @"[A-Z]+-\d+\s.*</a>").ToString().Replace("</a>", "");
            v.Name = name;

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
            return Desc ? b.Date.CompareTo(a.Date) : a.Date.CompareTo(b.Date);
        }
    }

    enum SortType {
        VideoCode,
        VideoBirthday
    }
}
