namespace microservices.catalog.api.Options
{
    public class MongoOption
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
    }
}
