using MongoDB.Driver;
using CommerceHub.Domain.Entities;  // <-- Domain project contains Order & Product

namespace CommerceHub.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        // Infrastructure should only take values via constructor
        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
    }
}