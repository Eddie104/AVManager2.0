using MongoDB.Bson;

namespace AVManager2.avManager.model
{
    class Actress_old : Collection_old
    {

        public string Name { get; set; }

        public Actress(BsonDocument doc) : base(doc)
        {
            collectionName = "actress";

            this.Name = doc["name"].AsString;
        }
    }
}
