using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using libra.db.mongoDB;

namespace AVManager2.gg
{
    /// <summary>
    /// mongoDB的测试
    /// </summary>
    class MongoDBTest
    {
        public static string connectionString = "mongodb://localhost";
        //数据库名
        public static string databaseName = "TestDB";

        public MongoDBTest()
        {
            MongoDBHelper.connectionString = "mongodb://localhost";
        }

        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <returns></returns>
        public bool Insert()
        {
            return new Users() { Age = 123, Name = "TestNameA", Sex = "F" }.Insert();
        }

        /// <summary>
        ///  删除一条数据
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            return Users.Remove(new QueryDocument("name", "TestNameA"));
        }

        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <returns></returns>
        public bool Modify()
        {
            IMongoQuery iq = new QueryDocument("name", "TestNameA");
            IMongoUpdate iu = MongoDB.Driver.Builders.Update.Set("sex", "M").Set("age", 100);
            return Users.Update(iq, iu);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        public void Select()
        {
            //IMongoQuery iq = new QueryDocument("name", "TestNameA");
            IMongoQuery iq = Query.And(Query.GTE("age", 40), Query.Matches("name", "/^Test/"));//>40
            List<Users> userList = Users.Search(iq).ToList();

            foreach (Users item in userList)
            {
                Console.WriteLine(item.Name + " " + item.Sex);
            }
        }

    }

    public class Users
    {
        private static string tableUser = "Users";

        public Users() { }
        public Users(String name, Int32 age, String sex)
        {
            Name = name;
            Age = age;
            Sex = sex;
        }

        public String Name { get; set; }

        public Int32 Age { get; set; }

        public String Sex { get; set; }

        public Boolean Insert()
        {
            BsonDocument dom = new BsonDocument {
                { "name", Name },
                { "age", Age },
                {"sex",Sex }
            };
            return MongoDBHelper.Insert("TestDB", tableUser, dom);
        }

        public static IEnumerable<Users> Search(IMongoQuery query)
        {
            foreach (BsonDocument tmp in MongoDBHelper.Search("TestDB", tableUser, query))
                yield return new Users(tmp["name"].AsString, tmp["age"].AsInt32, tmp["sex"].AsString);
        }

        public static Boolean Remove(IMongoQuery query)
        {
            return MongoDBHelper.Remove("TestDB", tableUser, query);
        }

        public static Boolean Update(IMongoQuery query, IMongoUpdate new_doc)
        {
            return MongoDBHelper.Update("TestDB", tableUser, query, new_doc);
        }
    }
}

