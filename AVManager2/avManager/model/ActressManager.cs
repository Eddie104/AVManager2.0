using avManager.model.data;
using libra.db.mongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
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
            if (list != null)
            {
                foreach (BsonDocument doc in list)
                    this.actressList.Add(new Actress(doc));
            }
        }

        public Actress AddActress(string name, string alias, DateTime birthday, int height, int bust, int waist, int hip, string cup)
        {
            Actress a = new Actress(new ObjectId(ObjectIdGenerator.Generate()), name, alias, birthday, height, bust, waist, hip, cup);
            this.actressList.Add(a);
            a.NeedInsert = true;
            return a;
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

        public Actress GetActress(string name)
        {
            foreach (Actress actress in actressList)
            {
                if (actress.Name.Contains(name) || actress.Alias.Contains(name))
                {
                    return actress;
                }
            }
            return null;
        }

        public List<Actress> GetActressList()
        {
            return this.actressList;
        }

        public List<Actress> GetActressList(string name)
        {
            List<Actress> result = new List<Actress>();
            foreach (Actress actress in actressList)
            {
                if (actress.Name.Contains(name) || actress.Alias.Contains(name))
                {
                    result.Add(actress);
                }
            }
            return result;
        }

        //public List<Actress> GetActressList(int startIndex, int length)
        //{
        //    List<Actress> result = new List<Actress>();
        //    int endIndex = Math.Min(startIndex + length - 1, this.actressList.Count - 1);
        //    for (int i = startIndex; i <= endIndex; i++)
        //    {
        //        result.Add(this.actressList[i]);
        //    }
        //    return result;
        //}

        //public List<Actress> GetActressList(int minHeight, int maxHeight)
        //{
        //    List<Actress> result = new List<Actress>();
        //    foreach (Actress a in this.actressList)
        //    {
        //        if (a.Height >= minHeight && a.Height <= maxHeight)
        //        {
        //            result.Add(a);
        //        }
        //    }
        //    return result;
        //}

        public List<Actress> GetActressList(string nameKeyWord, int minHeight, int maxHeight)
        {
            List<Actress> result = new List<Actress>();
            List<Actress> actressList = string.IsNullOrEmpty(nameKeyWord) ? this.actressList : GetActressList(nameKeyWord);

            if (maxHeight > minHeight)
            {
                foreach (Actress a in actressList)
                {
                    if (a.Height >= minHeight && a.Height <= maxHeight)
                    {
                        result.Add(a);
                    }
                }
            }
            else
            {
                result = actressList;
            }
            return result;
        }

        //public Actress GetFirstActress()
        //{
        //    return actressList.Count > 0 ? actressList[0] : null;
        //}

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
