using libra.db.mongoDB;
using MongoDB.Bson;

namespace AVManager2.avManager.model
{
    public class Collection
    {
        protected string collectionName;

        public ObjectId ID { get; set; }

        public Collection(BsonDocument doc)
        {
            ID = doc["id"].AsObjectId;
        }

        public bool Insert(BsonDocument doc)
        {
            return MongoDBHelper.Insert(DBManager.DB_NAME, collectionName, doc);
        }
    }
}
