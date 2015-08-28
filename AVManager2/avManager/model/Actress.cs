using MongoDB.Bson;

namespace AVManager2.avManager.model
{
    class Actress : Collection
    {

        public string Name { get; set; }

        public Actress(BsonDocument doc) : base(doc)
        {
            collectionName = "actress";

            this.Name = doc["name"].AsString;
        }
    }
}
