using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommerceHub.Domain.Entities;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }
}