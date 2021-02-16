namespace gmSim_API.MongoDocuments
{
    public class GmSimDatabaseSettings : IGmSimDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; } 
    }
}