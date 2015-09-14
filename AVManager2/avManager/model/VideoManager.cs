using avManager.model.data;
using libra.db.mongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public void Init()
        {
            MongoCursor<BsonDocument> list = MongoDBHelper.Search(collectionName);
            if (list != null)
            {
                foreach (BsonDocument doc in list)
                    this.videoList.Add(new Video(doc));
            }
        }

        public Video AddVideo(string name)
        {
            return this.AddVideo(new Video(new ObjectId(ObjectIdGenerator.Generate()), name));
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

        public Video CreateVideo(string html)
        {
            Video v = new Video();
            v.TorrentList = new List<string>();
            v.ActressList = new List<ObjectId>();
            v.ClassList = new List<ObjectId>();
            string imgURL = Regex.Match(Regex.Match(html, "<img id=\\\"video_jacket_img\\\" src=\\\".* width=").ToString(), "http.*jpg").ToString();
            v.Cover = imgURL;
            //http://pics.dmm.co.jp/mono/movie/adult/118sga033/118sga033ps.jpg
            //http://pics.dmm.co.jp/mono/movie/adult/118sga033/118sga033pl.jpg

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
                if (item.Contains("识别码"))
                {
                    v.Code = tdList[i + 1].ToString().Replace("<td class=\"text\">", "").Replace("</td>", "");
                }
                else if (item.Contains("发行日期"))
                {
                    v.Date = DateTime.Parse(tdList[i + 1].ToString().Replace("<td class=\"text\">", "").Replace("</td>", ""));
                }
                else if (item.Contains("类别"))
                {
                    Regex regPlace = new Regex("<a href=\"vl_genre.php\\?g=\\w+\" rel=\"category tag\">");
                    var classList = regA.Matches(tdList[i + 1].ToString());
                    foreach (var classItem in classList)
                    {
                        v.ClassList.Add(ClassTypeManager.GetInstance().GetClassType(regPlace.Replace(classItem.ToString(), "").Replace("</a>", "")).ID);
                    }
                }
                else if (item.Contains("演员"))
                {
                    Actress a;
                    Regex regPlace = new Regex("<a href=\"vl_star.php\\?s=\\w+\" rel=\"tag\">");
                    var classList = regA.Matches(tdList[i + 1].ToString());
                    foreach (var classItem in classList)
                    {
                        a = ActressManager.GetInstance().GetActress(regPlace.Replace(classItem.ToString(), "").Replace("</a>", ""));
                        if (a != null)
                        {
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
}
