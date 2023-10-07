 namespace mongodb_dotnet_example.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string GamesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string TrainCollectionName { get; set; }
        public string ReservationCollectionName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string GamesCollectionName { get; set; }
        string ReservationCollectionName { get; set; }
        string TrainCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}