using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace avManager.model.data
{
    class Video : Data
    {

        /// <summary>
        /// 封面
        /// </summary>
        [DB(DBField = "cover", DataType = DataType.String)]
        public string Cover { get; set; }

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
        [DB(DBField = "class", DataType = DataType.IntList)]
        public List<int> ClassList { get; set; }

        /// <summary>
        /// 演员
        /// </summary>
        [DB(DBField = "actress", DataType = DataType.IntList)]
        public List<int> ActressList { get; set; }

        /// <summary>
        /// 种子地址
        /// </summary>
        [DB(DBField = "torrent", DataType = DataType.StringList)]
        public List<string> TorrentList { get; set; }

        public Video(BsonDocument bsonDocument) : base(bsonDocument) { }

        public Video(ObjectId id, string name) : base(id, name) { }

        public Video() { }

        public override string ToString()
        {
            return string.Format("ID = {0}, Name = {1}, Date = {2}", ID, Name, Date.ToString("yyyy-MM-dd"));
        }
    }
}
