using System.Collections.Generic;
using DnsClient.Protocol;
using Entities.Models;
using gmSim_API.MongoDocuments;
using Microsoft.AspNetCore.Http.Extensions;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class GameService
    {
        private readonly IMongoCollection<GameDocument> _game;

            public GameService(IGmSimDatabaseSettings settings)
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);

                _game = database.GetCollection<GameDocument>("Game");
            }

            public GameDocument Get() => _game.Find(record => true).SingleOrDefault();

            public bool NewSeason(int newSeason)
            {
                var current = _game.Find(record => true).SingleOrDefault();
                var filter = Builders<GameDocument>.Filter.Eq(record => record.Id, current.Id);
                var update = Builders<GameDocument>.Update.Set(record => record.Season, newSeason)
                    .Set(record => record.Week, 1)
                    .Set(record => record.NextWeekType, WeekType.RegularSeason);
                var result = _game.UpdateOne(filter, update);
                
                return result.ModifiedCount == 1;
            }
            
            public bool NewWeek(int newWeek)
            {
                var current = _game.Find(record => true).SingleOrDefault();
                var filter = Builders<GameDocument>.Filter.Eq(record => record.Id, current.Id);
                var update = Builders<GameDocument>.Update.Set(record => record.Week, newWeek);
                var result = _game.UpdateOne(filter, update);
                
                return result.ModifiedCount == 1;
            }

            public bool NextWeek()
            {
                throw new System.NotImplementedException();
            }
    }
}