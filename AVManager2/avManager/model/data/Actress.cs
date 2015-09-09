using MongoDB.Bson;
using MongoDB.Driver;

namespace avManager.model.data
{
    class Actress : Data
    {

        public Actress(ObjectId id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public override IMongoUpdate CreateMongoUpdate()
        {
            return MongoDB.Driver.Builders.Update.Set("name", Name);
        }

        public override BsonDocument CreateBsonDocument()
        {
            return new BsonDocument {
                { "_id", this.ID },
                { "name", Name }
            };
        }
    }
}
