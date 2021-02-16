using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
    public class FixturesDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        public string Type { get; set; }
        public string HomeTeamId { get; set; }
        public string AwayTeamId { get; set; }
        public bool Played { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}