using libra.db.mongoDB;
using MongoDB.Bson;

namespace AVManager2.avManager.model
{
    public class Collection_old
    {
        protected string collectionName;

        public ObjectId ID { get; set; }

        public Collection_old(BsonDocument doc)
        {
            ID = doc["id"].AsObjectId;
        }

        public bool Insert(BsonDocument doc)
        {
            return MongoDBHelper.Insert(DBManager_old.DB_NAME, collectionName, doc);
        }
    }
}
