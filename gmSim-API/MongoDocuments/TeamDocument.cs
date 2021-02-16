using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace gmSim_API.MongoDocuments
{
    public class TeamDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string TeamName { get; set; }
    }
}