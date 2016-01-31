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

        public Actress AddActress(string name, string alias, DateTime birthday, int height, int bust, int waist, int hip, string cup, string code)
        {
            Actress a = new Actress(new ObjectId(ObjectIdGenerator.Generate()), name, alias, birthday, height, bust, waist, hip, cup, code);
            actressList.Add(a);
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

        public Actress GetActress(string name, bool mathAll = false)
        {
            foreach (Actress actress in actressList)
            {
                if (mathAll)
                {
                    if (actress.Name == name) return actress;
                    var a = actress.Alias.Split(new char[] { '|' });
                    foreach(string s in a)
                    {
                        if (s == name) return actress;
                    }
                }
                else
                {
                    if (actress.Name.Contains(name) || actress.Alias.Contains(name))
                    {
                        return actress;
                    }
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

        public List<Actress> GetActressList(string nameKeyWord, int minHeight, int maxHeight, bool sortByScoreDesc)
        {
            List<Actress> result = new List<Actress>();
            List<Actress> actressList = string.IsNullOrEmpty(nameKeyWord) ? new List<Actress>(this.actressList.ToArray()) : GetActressList(nameKeyWord);

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
            result.Sort(new SortByScoreCamparer(sortByScoreDesc));
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

    class SortByScoreCamparer : IComparer<Actress>
    {

        public bool Desc { get; set; }

        public SortByScoreCamparer(bool desc)
        {
            Desc = desc;
        }

        public int Compare(Actress a, Actress b)
        {
            if (a.Score > b.Score)
            {
                return Desc ? -1 : 1;
            }
            else if (a.Score == b.Score)
            {
                return 0;
            }
            return Desc ? 1 : -1;
        }
    }
}
