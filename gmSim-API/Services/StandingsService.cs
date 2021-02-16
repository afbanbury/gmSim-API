using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Entities.Models;
using gmSim_API.MongoDocuments;
using Microsoft.Extensions.ObjectPool;
using Microsoft.VisualBasic;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class StandingsService
    {
        private readonly IMongoCollection<StandingsDocument> _standings;

        public StandingsService(IGmSimDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _standings = database.GetCollection<StandingsDocument>("Standings");
        }

        public List<StandingsDocument> Get() => _standings.Find(record => true).ToList();

        public StandingsDocument Get(string id) => _standings.Find(record => record.Id == id).FirstOrDefault();
        
        public StandingsDocument GetTeamStandings(string team, int season)
        {
            var teamFilter = Builders<StandingsDocument>.Filter.Eq(record => record.TeamId, team);
            var seasonFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Season, season);
            
            var overallFilter = teamFilter & seasonFilter;
            return _standings.Find(overallFilter).FirstOrDefault();
        }

        public StandingsDocument GetDivisionLeaders(string conference, string division, int season)
        {
            var conferenceFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Conference, conference);
            var divisionFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Division, division);
            var seasonFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Season, season);
            var rankFilter = Builders<StandingsDocument>.Filter.Eq(record => record.DivisionRank, 1);
            
            var overallFilter = conferenceFilter & divisionFilter & seasonFilter & rankFilter;
            return _standings.Find(overallFilter).FirstOrDefault();
        }

        public StandingsDocument Create(StandingsDocument standings)
        {
            _standings.InsertOne(standings);
            return standings;
        }
        
        public List<StandingsDocument> GetDivisionStandings(string conference, string division, int season)
        {
            var conferenceFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Conference, conference);
            var divisionFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Division, division);
            var seasonFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Season, season);
            var sortOrder = Builders<StandingsDocument>.Sort.Ascending("DivisionRank");
            var overallFilter = conferenceFilter & divisionFilter & seasonFilter;
            return _standings.Find(overallFilter).Sort(sortOrder).ToList();
        }

        public bool UpdateOverallStandings(string recordId, int win, int loss, int tie, int ptsFor, int ptsAgainst)
        {
            var overallRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();

            var wins = overallRecord.OverallWins + win;
            var losses = overallRecord.OverallLosses + loss;
            var ties = overallRecord.OverallTies + tie;
            var pct = (wins + 0.5M * ties) / (wins + losses + ties);
            
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, overallRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.OverallWins, wins)
                .Set(record => record.OverallLosses, losses)
                .Set(record => record.OverallTies, ties)
                .Set(record => record.OverallPointsFor, overallRecord.OverallPointsFor + ptsFor)
                .Set(record => record.OverallPointsAgainst, overallRecord.OverallPointsAgainst + ptsAgainst)
                .Set(record => record.OverallPct, pct);
            var result = _standings.UpdateOne(filter, update);
                
            return result.ModifiedCount == 1;
        }
        
        public bool UpdateHomeStandings(string recordId, int win, int loss, int tie)
        {
            var homeRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, homeRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.HomeWins, homeRecord.HomeWins + win)
                .Set(record => record.HomeLosses, homeRecord.HomeLosses + loss)
                .Set(record => record.HomeTies, homeRecord.HomeTies + tie);
            var result = _standings.UpdateOne(filter, update);
                
            return result.ModifiedCount == 1;
        }
        
        public bool UpdateAwayStandings(string recordId, int win, int loss, int tie)
        {
            var awayRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, awayRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.AwayWins, awayRecord.AwayWins + win)
                .Set(record => record.AwayLosses, awayRecord.AwayLosses + loss)
                .Set(record => record.AwayTies, awayRecord.AwayTies + tie);
            var result = _standings.UpdateOne(filter, update);
                
            return result.ModifiedCount == 1;
        }

        public bool UpdateScheduleWeight(string teamId, int season, int strength)
        {
            var teamFilter = Builders<StandingsDocument>.Filter.Eq(record => record.TeamId, teamId);
            var seasonFilter = Builders<StandingsDocument>.Filter.Eq(record => record.Season, season);
            
            var overallFilter = teamFilter & seasonFilter;
            
            var update = Builders<StandingsDocument>.Update.Set(record => record.ScheduleWeight, strength);
            var result = _standings.UpdateOne(overallFilter, update);
                
            return result.ModifiedCount == 1;
        }
        public bool UpdateDivisionStandings(string recordId, int win, int loss, int tie)
        {
            var divisionRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();
            var wins = divisionRecord.DivisionWins + win;
            var losses = divisionRecord.DivisionLosses + loss;
            var ties = divisionRecord.DivisionTies + tie;
            var pct = (wins + 0.5M * ties) / (wins + losses + ties);
            
            
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, divisionRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.DivisionWins, wins)
                .Set(record => record.DivisionLosses, losses)
                .Set(record => record.DivisionTies, ties)
                .Set(record => record.DivisionPct, pct);
            var result = _standings.UpdateOne(filter, update);
                
            return result.ModifiedCount == 1;
        }
        
        public bool UpdateConferenceStandings(string recordId, int win, int loss, int tie)
        {
            var conferenceRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();
            var wins = conferenceRecord.ConferenceWins + win;
            var losses = conferenceRecord.ConferenceLosses + loss;
            var ties = conferenceRecord.ConferenceTies + tie;
            var pct = (wins + 0.5M * ties) / (wins + losses + ties);
            
            
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, conferenceRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.ConferenceWins, wins)
                .Set(record => record.ConferenceLosses, losses)
                .Set(record => record.ConferenceTies, ties)
                .Set(record => record.ConferencePct, pct);
            var result = _standings.UpdateOne(filter, update);
                
            return result.ModifiedCount == 1;
        }

        public bool UpdateStreak(string recordId, char result)
        {
            var streakRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();
            char streak;
            int count;

            if (streakRecord.CurrentStreak == result)
            {
                streak = streakRecord.CurrentStreak;
                count = streakRecord.StreakLength + 1;
            }
            else
            {
                streak = result;
                count = 1;
            }
            
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, streakRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.CurrentStreak, streak)
                .Set(record => record.StreakLength, count);
            var updateResult = _standings.UpdateOne(filter, update);
                
            return updateResult.ModifiedCount == 1;
        }

        public bool UpdateLastFive(string recordId, char result)
        {
            var streakRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();

            char[] lastFive = new char[5];

            lastFive[0] = result;

            for (int i = 0; i < 4; i++)
            {
                lastFive[i + 1] = streakRecord.LastFive[i];
            }
            
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, streakRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.LastFive, lastFive);
            var updateResult = _standings.UpdateOne(filter, update);
                
            return updateResult.ModifiedCount == 1;
        }

        public bool UpdateDivisionRank(string recordId, int rank)
        {
            var rankRecord = _standings.Find(record => record.Id == recordId).SingleOrDefault();
            
            var filter = Builders<StandingsDocument>.Filter.Eq(record => record.Id, rankRecord.Id);
            var update = Builders<StandingsDocument>.Update.Set(record => record.DivisionRank, rank);
            var updateResult = _standings.UpdateOne(filter, update);
                
            return updateResult.ModifiedCount == 1;
        }

    }
}