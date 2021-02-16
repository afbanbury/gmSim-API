using System;
using System.Collections.Generic;
using Entities.Models;
using gmSim_API.MongoDocuments;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class FixturesService
    {
        private readonly IMongoCollection<FixturesDocument> _fixtures;

        public FixturesService(IGmSimDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _fixtures = database.GetCollection<FixturesDocument>("Fixtures");
        }

        public List<FixturesDocument> Get()
        {
            var sortOrder = Builders<FixturesDocument>.Sort.Descending("Season").Ascending("Week");
            return _fixtures.Find(record => true).Sort(sortOrder).ToList();
        }
        
        public List<FixturesDocument> GetThisSeasonsFixtures(int season)
        {
            var seasonFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Season, season);
            var sortOrder = Builders<FixturesDocument>.Sort.Ascending("Id");
            
            return _fixtures.Find(seasonFilter).Sort(sortOrder).ToList();
        }
        public List<FixturesDocument> GetThisWeeksFixtures(int season, int week)
        {
            var seasonFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Season, season);
            var weekFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Week, week);
            var sortOrder = Builders<FixturesDocument>.Sort.Ascending("Id");
            
            var overallFilter = seasonFilter & weekFilter;
            return _fixtures.Find(overallFilter).Sort(sortOrder).ToList();
        }
        
        public List<FixturesDocument> GetTeamFixturesForSeason(string team, int season)
        {
            var seasonFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Season, season);
            var homeFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.HomeTeamId, team);
            var awayFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.AwayTeamId, team);
            var sortOrder = Builders<FixturesDocument>.Sort.Ascending("Week");
            
            var overallFilter = seasonFilter & (homeFilter | awayFilter);
            return _fixtures.Find(overallFilter).Sort(sortOrder).ToList();
        }

        public FixturesDocument GetFinalsFixtures(int season, string conference)
        {
            var gameType = conference.ToUpper() + " FINAL";
            
            var typeFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Type, gameType);
            var seasonFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Season, season);
            
            var overallFilter = typeFilter & seasonFilter;
            
            return _fixtures.Find(overallFilter).FirstOrDefault();
        }
        
        public FixturesDocument Get(string id) => _fixtures.Find(record => record.Id == id).FirstOrDefault();
        
        public bool UpdateFixture(string fixtureId, int homeScore, int awayScore)
        {
            var fixture = _fixtures.Find(game => game.Id == fixtureId).SingleOrDefault();
            var filter = Builders<FixturesDocument>.Filter.Eq(game => game.Id, fixture.Id);
            var update = Builders<FixturesDocument>.Update.Set(game => game.Played, true)
                .Set(game => game.HomeScore, homeScore)
                .Set(game => game.AwayScore, awayScore);
            var result = _fixtures.UpdateOne(filter, update);
                
            return result.ModifiedCount == 1;
        }

        public bool UpdateFinals(int season, string conference, string homeTeamId, string awayTeamId)
        {
            var gameType = conference.ToUpper() + " FINAL";
            
            var typeFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Type, gameType);
            var seasonFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Season, season);
            
            var overallFilter = typeFilter & seasonFilter;
            
            var update = Builders<FixturesDocument>.Update.Set(fixture => fixture.HomeTeamId, homeTeamId)
                .Set(fixture => fixture.AwayTeamId, awayTeamId);
            var result = _fixtures.UpdateOne(overallFilter, update);
                
            return result.ModifiedCount == 1;
        }

        public bool UpdateChampionship(int season, string homeTeamId, string awayTeamId)
        {
            var gameType = "CHAMPIONSHIP";
            
            var typeFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Type, gameType);
            var seasonFilter = Builders<FixturesDocument>.Filter.Eq(fixture => fixture.Season, season);
            
            var overallFilter = typeFilter & seasonFilter;
            
            var update = Builders<FixturesDocument>.Update.Set(fixture => fixture.HomeTeamId, homeTeamId)
                .Set(fixture => fixture.AwayTeamId, awayTeamId);
            var result = _fixtures.UpdateOne(overallFilter, update);
                
            return result.ModifiedCount == 1;
        }
        
        public FixturesDocument Create(FixturesDocument fixture)
        {
            _fixtures.InsertOne(fixture);
            return fixture;
        }

    }
}