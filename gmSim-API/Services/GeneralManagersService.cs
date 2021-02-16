using Entities.Models;
using gmSim_API.MongoDocuments;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class GeneralManagersService
    {
        private readonly IMongoCollection<GeneralManagersDocument> _generalManagers;

        public GeneralManagersService(IGmSimDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _generalManagers = database.GetCollection<GeneralManagersDocument>("GeneralManagers");
        }

        public GeneralManagersDocument Get() => _generalManagers.Find(record => true).SingleOrDefault();
        
        public GeneralManagersDocument Get(string id) => _generalManagers.Find(record => record.Id == id).FirstOrDefault();
        public GeneralManagersDocument Create(GeneralManagersDocument generalManager)
        {
            _generalManagers.InsertOne(generalManager);
            return generalManager;
        }
    }
}