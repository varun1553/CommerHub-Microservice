using CommerceHub.Application.Interfaces;
using CommerceHub.Domain.Entities;
using CommerceHub.Infrastructure.Data;
using MongoDB.Driver;

namespace CommerceHub.Infrastructure.Repositories;

public class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public MongoProductRepository(MongoDbContext context)
    {
        _products = context.Products;
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    // Atomic decrement
    public async Task<bool> DecrementStockAsync(string productId, int quantity)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, productId),
            Builders<Product>.Filter.Gte(p => p.Stock, quantity)
        );

        var update = Builders<Product>.Update.Inc(p => p.Stock, -quantity);

        var result = await _products.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }

    // PATCH endpoint logic
    public async Task<bool> AdjustStockAsync(string productId, int quantity)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, productId),
            Builders<Product>.Filter.Gte(p => p.Stock, -quantity)
        );

        var update = Builders<Product>.Update.Inc(p => p.Stock, quantity);

        var result = await _products.UpdateOneAsync(filter, update);

        return result.ModifiedCount > 0;
    }
}