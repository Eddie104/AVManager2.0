using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Reflection;

namespace avManager.model.data
{
    class Actress : Data
    {

        public Actress(BsonDocument bsonDocument) : base(bsonDocument) { }
        
        public Actress(ObjectId id, string name) : base(id, name) { }

    }
}
