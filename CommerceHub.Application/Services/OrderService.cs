using CommerceHub.Application.Interfaces;
using CommerceHub.Domain.Entities;

namespace CommerceHub.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IMessagePublisher _publisher;

    public OrderService(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IMessagePublisher publisher)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _publisher = publisher;
    }

    public async Task<Order> CheckoutAsync(string productId, int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than zero.");

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            throw new Exception("Product not found.");

        var stockUpdated = await _productRepo.DecrementStockAsync(productId, quantity);

        if (!stockUpdated)
            throw new Exception("Insufficient stock.");

        var order = new Order
        {
            ProductId = productId,
            Quantity = quantity,
            TotalPrice = product.Price * quantity,
            Status = "Pending"
        };

        await _orderRepo.CreateAsync(order);

        await _publisher.PublishAsync("OrderCreated", order);

        return order;
    }

    public async Task<Order?> GetByIdAsync(string id)
        => await _orderRepo.GetByIdAsync(id);

    public async Task UpdateAsync(string id, Order updatedOrder)
    {
        var existing = await _orderRepo.GetByIdAsync(id);

        if (existing == null)
            throw new Exception("Order not found.");

        if (existing.Status == "Shipped")
            throw new Exception("Cannot update shipped order.");

        updatedOrder.Id = id;

        await _orderRepo.UpdateAsync(updatedOrder);
    }
}