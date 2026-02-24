using NUnit.Framework;
using Moq;
using CommerceHub.Application.Services;
using CommerceHub.Application.Interfaces;
using CommerceHub.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CommerceHub.Tests;

public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IMessagePublisher> _publisherMock = null!;
    private OrderService _service = null!;

    [SetUp]
    public void Setup()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _publisherMock = new Mock<IMessagePublisher>();

        _service = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _publisherMock.Object
        );
    }


    // Validation: Prevent negative quantity
    [Test]
    public void Checkout_ShouldThrowException_WhenQuantityIsNegative()
    {
        Assert.ThrowsAsync<Exception>(async () =>
        {
            await _service.CheckoutAsync("prod-1", -5);
        });
    }

    // Stock Decrement Logic
    [Test]
    public async Task Checkout_ShouldDecrementStock_AndCreateOrder()
    {
        // Arrange
        var product = new Product
        {
            Id = "prod-1",
            Price = 10,
            Stock = 10
        };

        _productRepoMock
            .Setup(x => x.GetByIdAsync("prod-1"))
            .ReturnsAsync(product);

        _productRepoMock
            .Setup(x => x.DecrementStockAsync("prod-1", 2))
            .ReturnsAsync(true);

        // Act
        var order = await _service.CheckoutAsync("prod-1", 2);

        // Assert
        Assert.That(order.Quantity, Is.EqualTo(2));
        Assert.That(order.TotalPrice, Is.EqualTo(20));
        Assert.That(order.Status, Is.EqualTo("Pending"));

        _productRepoMock.Verify(x =>
            x.DecrementStockAsync("prod-1", 2),
            Times.Once);

        _orderRepoMock.Verify(x =>
            x.CreateAsync(It.IsAny<Order>()),
            Times.Once);
    }

    // Insufficient Stock Scenario
    [Test]
    public void Checkout_ShouldThrowException_WhenStockIsInsufficient()
    {
        var product = new Product
        {
            Id = "prod-1",
            Price = 10,
            Stock = 2
        };

        _productRepoMock
            .Setup(x => x.GetByIdAsync("prod-1"))
            .ReturnsAsync(product);

        _productRepoMock
            .Setup(x => x.DecrementStockAsync("prod-1", 5))
            .ReturnsAsync(false);

        Assert.ThrowsAsync<Exception>(async () =>
        {
            await _service.CheckoutAsync("prod-1", 5);
        });
    }

    // Event Emission Verification
    [Test]
    public async Task Checkout_ShouldPublishOrderCreatedEvent()
    {
        var product = new Product
        {
            Id = "prod-1",
            Price = 15,
            Stock = 10
        };

        _productRepoMock
            .Setup(x => x.GetByIdAsync("prod-1"))
            .ReturnsAsync(product);

        _productRepoMock
            .Setup(x => x.DecrementStockAsync("prod-1", 1))
            .ReturnsAsync(true);

        await _service.CheckoutAsync("prod-1", 1);

        _publisherMock.Verify(p =>
            p.PublishAsync("OrderCreated", It.IsAny<object>()),
            Times.Once);
    }
}