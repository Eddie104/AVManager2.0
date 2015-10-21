using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace avManager.model.data
{
    public class Video : Data
    {

        ///// <summary>
        ///// 封面
        ///// </summary>
        //[DB(DBField = "cover", DataType = DataType.String)]
        //public string Cover { get; set; }

        /// <summary>
        /// 番号
        /// </summary>
        [DB(DBField = "code", DataType = DataType.String)]
        public string Code { get; set; }

        /// <summary>
        /// 发行日期
        /// </summary>
        [DB(DBField = "date", DataType = DataType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [DB(DBField = "class", DataType = DataType.ObjectIdList)]
        public List<ObjectId> ClassList { get; set; }

        /// <summary>
        /// 演员
        /// </summary>
        [DB(DBField = "actress", DataType = DataType.ObjectIdList)]
        public List<ObjectId> ActressList { get; set; }

        /// <summary>
        /// 种子地址
        /// </summary>
        [DB(DBField = "torrent", DataType = DataType.StringList)]
        public List<string> TorrentList { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [DB(DBField ="score", DataType = DataType.Int32)]
        public int Score { get; set; }

        /// <summary>
        /// 是否已下载
        /// </summary>
        [DB(DBField ="hasDownload", DataType = DataType.boolean)]
        public bool HasDownload { get; set; }

        /// <summary>
        /// 封面的缩略图的地址
        /// </summary>
        public string SubImgUrl { get; set; }

        /// <summary>
        /// 封面地址
        /// </summary>
        private string imgUrl = "";
        public string ImgUrl
        {
            get { return imgUrl; }
            set
            {
                imgUrl = value;
                SubImgUrl = imgUrl.Replace("pl.jpg", "ps.jpg");
            }
        }

        public Video(BsonDocument bsonDocument) : base(bsonDocument) { }

        public Video(ObjectId id, string name) : base(id, name) { }

        public Video() { }

        public string GetClassString()
        {
            ClassTypeManager m = ClassTypeManager.GetInstance();
            string s = "";
            foreach (ObjectId classTypeID in ClassList)
            {
                s += m.GetClassType(classTypeID).Name + " ";
            }
            return s;
        }

        public string GetActressString()
        {
            ActressManager m = ActressManager.GetInstance();
            string s = "";
            foreach (ObjectId actressID in ActressList)
            {
                s += m.GetActress(actressID).Name + "";
            }
            return s;
        }

        public bool HasActress(ObjectId actressID)
        {
            foreach (ObjectId id in ActressList)
            {
                if (id == actressID)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("ID = {0}, Name = {1}, Date = {2}", ID, Name, Date.ToString("yyyy-MM-dd"));
        }
    }
}
