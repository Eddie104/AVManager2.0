using libra.db.mongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace AVManager2.avManager.model
{
    class DBManager_old
    {
        public const string DB_NAME = "avdb";

        public static bool Remove(string collectionName, IMongoQuery query)
        {
            return MongoDBHelper.Remove(DBManager_old.DB_NAME, collectionName, query);
        }

        public static bool Update(string collectionName, IMongoQuery query, IMongoUpdate new_doc)
        {
            return MongoDBHelper.Update(DBManager_old.DB_NAME, collectionName, query, new_doc);
        }

        public static IEnumerable<Collection_old> Search(string collectionName, IMongoQuery query = null)
        {
            foreach (BsonDocument tmp in MongoDBHelper.Search(DBManager_old.DB_NAME, collectionName, query))
                //yield return new Collection(tmp["name"].AsString, tmp["age"].AsInt32, tmp["sex"].AsString);
                yield return new Collection_old(tmp);
        }
    }
}
