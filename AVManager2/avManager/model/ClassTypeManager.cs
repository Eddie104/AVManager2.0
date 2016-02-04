using avManager.model.data;
using libra.db.mongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace avManager.model
{
    class ClassTypeManager
    {
        private static ClassTypeManager instance = null;

        private readonly string collectionName;

        private List<ClassType> classTypeList = new List<ClassType>();

        private ClassTypeManager()
        {
            collectionName = "classType";
        }

        public void Init(Action callback)
        {
            MongoCursor<BsonDocument> list = MongoDBHelper.Search(collectionName);
            if (list != null)
            {
                foreach (BsonDocument doc in list)
                    this.classTypeList.Add(new ClassType(doc));
            }
            callback();
        }

        /// <summary>
        /// 获取重复的classType
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<ClassType>> GetRepeatClassType()
        {
            Dictionary<string, List<ClassType>> result = new Dictionary<string, List<ClassType>>();
            foreach (var item in classTypeList)
            {
                if (this.classTypeList.FindAll(c => c.Name == item.Name).Count > 1)
                {
                    List<ClassType> list = null;
                    if (result.ContainsKey(item.Name))
                    {
                        list = result[item.Name];
                    }
                    else
                    {
                        list = new List<ClassType>();
                        result[item.Name] = list;
                    }
                    list.Add(item);
                }
            }
            return result;
        }

        public ClassType AddTypeClass(string name)
        {
            ClassType classType = new ClassType(new ObjectId(ObjectIdGenerator.Generate()), name);
            this.classTypeList.Add(classType);
            classType.NeedInsert = true;
            return classType;
        }

        public ClassType RemoveClassType(ObjectId id)
        {
            foreach (ClassType classType in classTypeList)
            {
                if (classType.ID == id)
                {
                    classType.NeedDelete = true;
                    return classType;
                }
            }
            return null;
        }

        public void UpdateClassType(ObjectId id, Dictionary<string, object> property)
        {
            ClassType classType = this.GetClassType(id);
            if (classType != null)
            {
                classType.Update(property);
                classType.NeedUpdate = true;
            }
        }

        public ClassType GetClassType(ObjectId id)
        {
            foreach (ClassType classType in classTypeList)
            {
                if (classType.ID == id)
                {
                    return classType;
                }
            }
            return null;
        }

        public ClassType GetClassType(string name, bool autoCreate = true)
        {
            foreach (ClassType classType in classTypeList)
            {
                if (classType.Name == name)
                {
                    return classType;
                }
            }
            if (autoCreate)
            {
                return this.AddTypeClass(name);
            }
            return null;
        }

        public List<ClassType> GetClassType()
        {
            return new List<ClassType>(this.classTypeList.ToArray());
        }

        public void SaveToDB(Action callback)
        {
            List<ClassType> tmp = new List<ClassType>(classTypeList.ToArray());
            foreach (ClassType classType in tmp)
            {
                if (classType.NeedDelete)
                {
                    if (MongoDBHelper.Remove(collectionName, new QueryDocument("_id", classType.ID)))
                    {
                        classTypeList.Remove(classType);
                    }
                }
                else if (classType.NeedUpdate)
                {
                    if (MongoDBHelper.Update(collectionName, new QueryDocument("_id", classType.ID), classType.CreateMongoUpdate()))
                    {
                        classType.NeedUpdate = false;
                    }
                }
                else if (classType.NeedInsert)
                {
                    if (MongoDBHelper.Insert(collectionName, classType.CreateBsonDocument()))
                    {
                        classType.NeedInsert = false;
                    }
                }
            }
            callback();
        }

        public static ClassTypeManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ClassTypeManager();
            }
            return instance;
        }
    }
}
