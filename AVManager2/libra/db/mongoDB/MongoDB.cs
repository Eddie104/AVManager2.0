using MongoDB.Bson;
using MongoDB.Driver;

namespace libra.db.mongoDB
{
    class MongoDBHelper
    {
        public static string connectionString;

        public static string dbName = null;

        private static MongoServer server;

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="collectionName">表名</param>
        /// <param name="query">查找条件</param>
        /// <returns>查找的结果</returns>
        public static MongoCursor<BsonDocument> Search(string collectionName, IMongoQuery query = null)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, out collection);
            if (server != null && collection != null)
            {
                try
                {
                    return query == null ? collection.FindAll() : collection.Find(query);
                }
                catch
                {
                    return null;
                }
                //finally
                //{
                //    server.Disconnect();
                //}
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 插入一条新的数据
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static bool Insert(string collectionName, BsonDocument document)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, out collection);
            try
            {
                collection.Insert(document);
                //server.Disconnect();
                return true;
            }
            catch
            {
                //server.Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>  
        public static bool Update(string collectionName, IMongoQuery query, IMongoUpdate newDoc)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, out collection);
            try
            {
                collection.Update(query, newDoc);
                //server.Disconnect();
                return true;
            }
            catch
            {
                //server.Disconnect();
                return false;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        public static bool Remove(string collectionName, IMongoQuery query = null)
        {
            MongoCollection<BsonDocument> collection;
            MongoServer server = CreateMongoServer(collectionName, out collection);
            bool ok = false;
            try
            {
                ok = collection.Remove(query).Ok;
            }
            catch
            {
                ok = false;
            }
            //finally
            //{
            //    server.Disconnect();
            //}
            return ok;
        }

        public static void Exit()
        {
            if (server != null && server.State == MongoServerState.Connected)
            {
                server.Disconnect();
            }            
        }

        private static MongoServer CreateMongoServer(string collectionName, out MongoCollection<BsonDocument> collection)
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
                    try
                    {
                        server.Connect();
                    }
                    catch (System.Exception)
                    {
                        collection = null;
                        return null;
                    }
                }
                if (server.State == MongoServerState.Connected)
                {
                    if (!string.IsNullOrEmpty(dbName))
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
                else
                {
                    collection = null;
                    return null;
                }
            }
        }
    }
}
