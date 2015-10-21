using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace avManager.model.data
{

    enum DataType
    {
        ObjectId,
        Int32,
        String,
        Date,
        ObjectIdList,
        IntList,
        StringList,
        boolean
    }

    class DBAttribute : Attribute
    {
        public string DBField { get; set; }

        public DataType DataType { get; set; }
    }

    public class Data
    {
        [DB(DBField = "_id", DataType = DataType.ObjectId)]
        public ObjectId ID { get; set; }

        [DB(DBField = "name", DataType = DataType.String)]
        public string Name { get; set; }

        public bool NeedInsert { get; set; }

        public bool NeedUpdate { get; set; }

        public bool NeedDelete { get; set; }

        public Data() { }

        public Data(BsonDocument bsonDocument)
        {
            DBAttribute dbAttribute = null;
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                foreach (var attribute in propertyInfo.GetCustomAttributes())
                {
                    if (attribute is DBAttribute)
                    {
                        dbAttribute = attribute as DBAttribute;
                        if (bsonDocument.Contains(dbAttribute.DBField))
                        {
                            if (dbAttribute.DataType == DataType.ObjectId)
                            {
                                propertyInfo.SetValue(this, bsonDocument[dbAttribute.DBField].AsObjectId);
                            }
                            else if (dbAttribute.DataType == DataType.Int32)
                            {
                                propertyInfo.SetValue(this, bsonDocument[dbAttribute.DBField].AsInt32);
                            }
                            else if (dbAttribute.DataType == DataType.String)
                            {
                                propertyInfo.SetValue(this, bsonDocument[dbAttribute.DBField].IsBsonNull ? "" : bsonDocument[dbAttribute.DBField].AsString);
                            }
                            else if(dbAttribute.DataType == DataType.boolean)
                            {
                                propertyInfo.SetValue(this, bsonDocument[dbAttribute.DBField].AsBoolean);
                            }
                            else if (dbAttribute.DataType == DataType.Date)
                            {
                                propertyInfo.SetValue(this, bsonDocument[dbAttribute.DBField].IsBsonNull ? new DateTime(1980, 1, 1) : bsonDocument[dbAttribute.DBField].ToUniversalTime());
                            }
                            else if (dbAttribute.DataType == DataType.IntList)
                            {
                                List<int> list = null;
                                if (!bsonDocument[dbAttribute.DBField].IsBsonNull)
                                {
                                    list = new List<int>();
                                    foreach (var val in bsonDocument[dbAttribute.DBField].AsBsonArray.ToList())
                                    {
                                        list.Add(val.AsInt32);
                                    }
                                }
                                propertyInfo.SetValue(this, list);
                            }
                            else if (dbAttribute.DataType == DataType.StringList)
                            {
                                List<string> list = null;
                                if (!bsonDocument[dbAttribute.DBField].IsBsonNull)
                                {
                                    list = new List<string>();
                                    foreach (var val in bsonDocument[dbAttribute.DBField].AsBsonArray.ToList())
                                    {
                                        list.Add(val.AsString);
                                    }
                                }
                                propertyInfo.SetValue(this, list);
                            }
                            else if (dbAttribute.DataType == DataType.ObjectIdList)
                            {
                                List<ObjectId> list = null;
                                if (!bsonDocument[dbAttribute.DBField].IsBsonNull)
                                {
                                    list = new List<ObjectId>();
                                    foreach (var val in bsonDocument[dbAttribute.DBField].AsBsonArray.ToList())
                                    {
                                        list.Add(val.AsObjectId);
                                    }
                                }
                                propertyInfo.SetValue(this, list);
                            }
                            this.NeedUpdate = true;
                        }
                        else
                        {
                            this.NeedUpdate = true;
                        }
                    }
                }
            }
        }

        public Data(ObjectId id, string name)
        {
            ID = id;
            Name = name;
        }

        public void Update(Dictionary<string, object> propertyDic)
        {
            //获取类型
            Type type = this.GetType();
            PropertyInfo propertyInfo = null;
            foreach (var item in propertyDic)
            {
                //获取指定名称的属性
                propertyInfo = type.GetProperty(item.Key);
                if (propertyInfo != null)
                {
                    //给对应属性赋值
                    propertyInfo.SetValue(this, item.Value);
                }
            }
        }

        public IMongoUpdate CreateMongoUpdate()
        {
            UpdateBuilder updateBuilder = null;
            foreach (var item in GetDBKeyVal())
            {
                if (updateBuilder == null)
                {
                    updateBuilder = MongoDB.Driver.Builders.Update.Set(item.Key, BsonValue.Create(item.Value));
                }
                else
                {
                    updateBuilder.Set(item.Key, BsonValue.Create(item.Value));
                }
            }
            return updateBuilder;
        }

        public BsonDocument CreateBsonDocument()
        {
            BsonDocument bsonDocument = new BsonDocument();
            bsonDocument.AddRange(GetDBKeyVal());
            return bsonDocument;
        }

        public override string ToString()
        {
            return string.Format("ID = {0}, Name = {1}", ID, Name);
        }

        private Dictionary<string, object> GetDBKeyVal()
        {
            Dictionary<string, object> keyVal = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                foreach (var attribute in propertyInfo.GetCustomAttributes())
                {
                    if (attribute is DBAttribute)
                    {
                        keyVal.Add((attribute as DBAttribute).DBField, propertyInfo.GetValue(this));
                    }
                }
            }
            return keyVal;
        }
    }
}
