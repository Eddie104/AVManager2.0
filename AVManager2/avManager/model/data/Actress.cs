using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace avManager.model.data
{
    public class Actress : Data
    {
        /// <summary>
        /// 别名
        /// </summary>
        [DB(DBField = "alias", DataType = DataType.String)]
        public string Alias { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [DB(DBField = "birthday", DataType = DataType.Date)]
        public DateTime Birthday{ get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        [DB(DBField = "height", DataType = DataType.Int32)]
        public int Height { get; set; }

        /// <summary>
        /// 胸围
        /// </summary>
        [DB(DBField = "bust", DataType = DataType.Int32)]
        public int Bust { get; set; }

        /// <summary>
        /// 腰围
        /// </summary>
        [DB(DBField = "waist", DataType = DataType.Int32)]
        public int Waist { get; set; }

        /// <summary>
        /// 臀围
        /// </summary>
        [DB(DBField = "hip", DataType = DataType.Int32)]
        public int Hip { get; set; }

        /// <summary>
        /// 罩杯
        /// </summary>
        [DB(DBField = "cup", DataType = DataType.String)]
        public string Cup { get; set; }
        
        /// <summary>
        /// 评分
        /// </summary>
        [DB(DBField = "score", DataType = DataType.Int32)]
        public int Score { get; set; }

        /// <summary>
        /// 图片名
        /// </summary>
        [DB(DBField = "code", DataType = DataType.String)]
        public string Code { get; set; }

        public Actress(BsonDocument bsonDocument) : base(bsonDocument) { }

        public Actress(ObjectId id, string name, string alias, DateTime birthday, int height, int bust, int waist, int hip, string cup)
        {
            ID = id;
            Name = name;
            Alias = alias;
            Birthday = birthday;
            Height = height;
            Bust = bust;
            Waist = waist;
            Hip = hip;
            Cup = cup;
        }

    }
}
