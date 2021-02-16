using Entities.Models;
using gmSim_API.MongoDocuments;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class PlayersService
    {
        private readonly IMongoCollection<PlayersDocument> _players;

        public PlayersService(IGmSimDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _players = database.GetCollection<PlayersDocument>("Players");
        }

        public PlayersDocument Get() => _players.Find(record => true).SingleOrDefault();
        
        public PlayersDocument Get(string id) => _players.Find(record => record.Id == id).FirstOrDefault();
        
        public PlayersDocument Create(PlayersDocument player)
        {
            _players.InsertOne(player);
            return player;
        }
    }
}