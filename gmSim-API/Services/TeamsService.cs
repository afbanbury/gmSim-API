using System.Collections.Generic;
using DnsClient.Protocol;
using Entities.Models;
using gmSim_API.MongoDocuments;
using MongoDB.Driver;

namespace gmSim_API.Services
{
    public class TeamsService
    {
        private readonly IMongoCollection<TeamsDocument> _teams;

        public TeamsService(IGmSimDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _teams = database.GetCollection<TeamsDocument>("Teams");
        }

        public List<TeamsDocument> Get() => _teams.Find(record => true).ToList();

        public List<TeamsDocument> GetConferenceTeams(string conference) =>
            _teams.Find(record => record.Conference == conference).ToList();
        
        public List<TeamsDocument> GetDivisionTeams(string conference, string division)
        {
            var conferenceFilter = Builders<TeamsDocument>.Filter.Eq(record => record.Conference, conference);
            var divisionFilter = Builders<TeamsDocument>.Filter.Eq(record => record.Division, division);
            var conferenceAndDivisionFilter = conferenceFilter & divisionFilter;
            return _teams.Find(conferenceAndDivisionFilter).ToList();
        }

        public TeamsDocument Get(string id) => _teams.Find(record => record.Id == id).FirstOrDefault();
        
        public TeamsDocument Create(TeamsDocument team)
        {
            _teams.InsertOne(team);
            return team;
        }
    }
}