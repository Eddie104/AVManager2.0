using avManager.model.data;
using libra.db.mongoDB;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avManager.model
{
    class ActressManager
    {
        public List<Actress> ActressList { get; set; }

        public void Init()
        {
            foreach (BsonDocument doc in MongoDBHelper.Search("avdb", "actress"))
                //yield return new Collection(tmp["name"].AsString, tmp["age"].AsInt32, tmp["sex"].AsString);
                this.AddActress(doc["id"].AsObjectId, doc["name"].AsString);
        }

        public void AddActress(ObjectId id, string name, bool isNew = false)
        {
            Actress a = new Actress(id, name);
            this.ActressList.Add(a);
            a.NeedInsert = isNew;
        }

        public Actress RemoveActress(ObjectId id)
        {
            foreach (Actress actress in ActressList)
            {
                if (actress.ID == id)
                {
                    ActressList.Remove(actress);
                    actress.NeedDelete = true;
                    return actress;
                }
            }
            return null;
        }

        public Actress GetActress(ObjectId id)
        {
            foreach (Actress actress in ActressList)
            {
                if (actress.ID == id)
                {
                    return actress;
                }
            }
            return null;
        }
    }
}
