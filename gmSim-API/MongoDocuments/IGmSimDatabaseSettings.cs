namespace gmSim_API.MongoDocuments
{
    public interface IGmSimDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; } 
    }
}