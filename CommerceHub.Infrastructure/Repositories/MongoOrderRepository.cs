using CommerceHub.Application.Interfaces;
using CommerceHub.Domain.Entities;
using CommerceHub.Infrastructure.Data;
using MongoDB.Driver;

namespace CommerceHub.Infrastructure.Repositories;

public class MongoOrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public MongoOrderRepository(MongoDbContext context)
    {
        _orders = context.Orders;
    }

    public async Task CreateAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        return await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
    }
}