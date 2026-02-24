using CommerceHub.Domain.Entities;

namespace CommerceHub.Application.Interfaces;

public interface IOrderRepository
{
    Task CreateAsync(Order order);
    Task<Order?> GetByIdAsync(string id);
    Task UpdateAsync(Order order);
}