using CommerceHub.Domain.Entities;

namespace CommerceHub.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id);
    Task<bool> DecrementStockAsync(string productId, int quantity);
    Task<bool> AdjustStockAsync(string productId, int quantity);
}