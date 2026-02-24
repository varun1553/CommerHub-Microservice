using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommerceHub.Domain.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ProductId { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = "Pending";
}