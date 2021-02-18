using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
    public class GameDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<string> Conferences { get; set; }
        public List<string> Divisions { get; set; }
        public int Season { get; set; }
        public int Week { get; set; }
        
        [BsonRepresentation(BsonType.String)]  
        public WeekType NextWeekType { get; set; }
    }

    public enum WeekType
    {
        RegularSeason,
        DivisionalFinals,
        Championship,
        OffSeason, 
        EndSeason
    }
}