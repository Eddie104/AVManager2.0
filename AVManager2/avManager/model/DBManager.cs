using libra.db.mongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace AVManager2.avManager.model
{
    class DBManager
    {
        public const string DB_NAME = "avdb";

        public static bool Remove(string collectionName, IMongoQuery query)
        {
            return MongoDBHelper.Remove(DBManager.DB_NAME, collectionName, query);
        }

        public static bool Update(string collectionName, IMongoQuery query, IMongoUpdate new_doc)
        {
            return MongoDBHelper.Update(DBManager.DB_NAME, collectionName, query, new_doc);
        }

        public static IEnumerable<Collection> Search(string collectionName, IMongoQuery query = null)
        {
            foreach (BsonDocument tmp in MongoDBHelper.Search(DBManager.DB_NAME, collectionName, query))
                //yield return new Collection(tmp["name"].AsString, tmp["age"].AsInt32, tmp["sex"].AsString);
                yield return new Collection(tmp);
        }
    }
}
