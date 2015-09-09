using avManager.model.data;
using libra.db.mongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace avManager.model
{
    class ActressManager
    {

        private static ActressManager instance = null;

        private readonly string collectionName;

        private List<Actress> actressList = new List<Actress>();

        private ActressManager()
        {
            collectionName = "actress";
        }

        public void Init()
        {
            MongoCursor<BsonDocument> list = MongoDBHelper.Search(collectionName);
            foreach (BsonDocument doc in list)
                this.actressList.Add(new Actress(doc["_id"].AsObjectId, doc["name"].AsString));
        }

        public void AddActress(string name)
        {
            Actress a = new Actress(new ObjectId(ObjectIdGenerator.Generate()), name);
            this.actressList.Add(a);
            a.NeedInsert = true;
        }

        public Actress RemoveActress(ObjectId id)
        {
            foreach (Actress actress in actressList)
            {
                if (actress.ID == id)
                {
                    actress.NeedDelete = true;
                    return actress;
                }
            }
            return null;
        }

        public void UpdateActress(ObjectId id, Dictionary<string, object> property)
        {
            Actress a = this.GetActress(id);
            if (a != null)
            {
                a.Update(property);
                a.NeedUpdate = true;
            }
        }

        public Actress GetActress(ObjectId id)
        {
            foreach (Actress actress in actressList)
            {
                if (actress.ID == id)
                {
                    return actress;
                }
            }
            return null;
        }

        public void SaveToDB()
        {
            List<Actress> tmp = new List<Actress>(actressList.ToArray());
            foreach (Actress actress in tmp)
            {
                if (actress.NeedDelete)
                {
                    if (MongoDBHelper.Remove(collectionName, new QueryDocument("_id", actress.ID)))
                    {
                        actressList.Remove(actress);
                    }
                }
                else if (actress.NeedUpdate)
                {
                    if (MongoDBHelper.Update(collectionName, new QueryDocument("_id", actress.ID), actress.CreateMongoUpdate()))
                    {
                        actress.NeedUpdate = false;
                    }
                }
                else if (actress.NeedInsert)
                {
                    if (MongoDBHelper.Insert(collectionName, actress.CreateBsonDocument()))
                    {
                        actress.NeedInsert = false;
                    }
                }
            }
        }

        public static ActressManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ActressManager();
            }
            return instance;
        }
    }
}
