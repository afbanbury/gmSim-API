using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
    public class StandingsDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Season { get; set; }
        public string TeamId { get; set; }
        public string Conference { get; set; }
        public string Division { get; set; }
        public int DivisionRank { get; set; }
        public int ConferenceRank { get; set; }
        public int OverallRank { get; set; }
        public int OverallWins { get; set; }
        public int OverallLosses { get; set; }
        public int OverallTies { get; set; }
        public decimal OverallPct { get; set; }
        public int OverallPointsFor { get; set; }
        public int OverallPointsAgainst { get; set; }
        public int HomeWins { get; set; }
        public int HomeLosses { get; set; }
        public int HomeTies { get; set; }
        public int AwayWins { get; set; }
        public int AwayLosses { get; set; }
        public int AwayTies { get; set; }
        public int DivisionWins { get; set; }
        public int DivisionLosses { get; set; }
        public int DivisionTies { get; set; }
        public decimal DivisionPct { get; set; }
        public int ConferenceWins { get; set; }
        public int ConferenceLosses { get; set; }
        public int ConferenceTies { get; set; }
        public decimal ConferencePct { get; set; }
        public char CurrentStreak { get; set; }
        public int StreakLength { get; set; }
        public char[] LastFive { get; set; }
        
        public int ScheduleWeight { get; set; }
    }
}