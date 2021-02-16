using Entities.Models;
using gmSim_API.MongoDocuments;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class CoachesService
    {
        private readonly IMongoCollection<CoachesDocument> _coaches;

        public CoachesService(IGmSimDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _coaches = database.GetCollection<CoachesDocument>("Coaches");
        }

        public CoachesDocument Get() => _coaches.Find(record => true).SingleOrDefault();
        
        public CoachesDocument Get(string id) => _coaches.Find(record => record.Id == id).FirstOrDefault();
        public CoachesDocument Create(CoachesDocument coach)
        {
            _coaches.InsertOne(coach);
            return coach;
        }
    }
}