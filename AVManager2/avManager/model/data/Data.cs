using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace avManager.model.data
{
    class Data
    {
        public ObjectId ID { get; set; }

        public string Name { get; set; }

        public bool NeedInsert { get; set; }

        public bool NeedUpdate { get; set; }

        public bool NeedDelete { get; set; }

        public void Update(Dictionary<string, object> property)
        {
            //获取类型
            Type type = this.GetType();
            PropertyInfo propertyInfo = null;
            foreach (var item in property)
            {
                //获取指定名称的属性
                propertyInfo = type.GetProperty(item.Key);
                if (propertyInfo != null)
                {
                    //给对应属性赋值
                    propertyInfo.SetValue(this, item.Value, null);
                }
            }
        }

        public virtual IMongoUpdate CreateMongoUpdate()
        {
            throw new NotImplementedException();
        }

        public virtual BsonDocument CreateBsonDocument()
        {
            throw new NotImplementedException();
        }
    }
}
