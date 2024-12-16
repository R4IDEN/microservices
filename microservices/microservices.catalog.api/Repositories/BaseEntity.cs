using MongoDB.Bson.Serialization.Attributes;

namespace microservices.catalog.api.Repositories
{
    public class BaseEntity
    {
        [BsonElement("_id")] public Guid Id { get; set; }
    }
}
