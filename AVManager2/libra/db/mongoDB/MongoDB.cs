using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace libra.db.mongoDB
{
    class MongoDBHelper
    {
        public static string connectionString;

        private static MongoServer server;

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <param name="collectionName">表名</param>
        /// <param name="query">查找条件</param>
        /// <returns>查找的结果</returns>
        public static MongoCursor<BsonDocument> Search(string dbName, string collectionName, IMongoQuery query)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, dbName, out collection);
            if (server != null && collection != null)
            {
                try
                {
                    return query == null ? collection.FindAll() : collection.Find(query);
                }
                finally
                {
                    server.Disconnect();
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 插入一条新的数据
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static bool Insert(string dbName, string collectionName, BsonDocument document)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, dbName, out collection);
            try
            {
                collection.Insert(document);
                server.Disconnect();
                return true;
            }
            catch
            {
                server.Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>  
        public static bool Update(string dbName, string collectionName, IMongoQuery query, IMongoUpdate newDoc)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, dbName, out collection);
            try
            {
                collection.Update(query, newDoc);
                server.Disconnect();
                return true;
            }
            catch
            {
                server.Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        public static bool Remove(string dbName, string collectionName, IMongoQuery query)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, dbName, out collection);
            bool ok = false;
            try
            {
                ok = collection.Remove(query).Ok;
            }
            finally
            {
                server.Disconnect();
            }
            return ok;
        }

        private static MongoServer CreateMongoServer(string collectionName, string dbName, out MongoCollection<BsonDocument> collection)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                collection = null;
                return null;
            }
            else
            {
                if (server == null)
                {
                    server = new MongoClient(connectionString).GetServer();
                }
                if (server.State == MongoServerState.Connecting)
                {
                    collection = server.GetDatabase(dbName).GetCollection<BsonDocument>(collectionName);
                    return server;
                }
                else
                {
                    collection = null;
                    return null;
                }
            }
        }
    }
}
