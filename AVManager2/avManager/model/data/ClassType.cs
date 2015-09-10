using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avManager.model.data
{
    public class ClassType : Data
    {

        public ClassType(BsonDocument bsonDocument) : base(bsonDocument) { }

        public ClassType(ObjectId id, string name) : base(id, name) { }

    }
}
