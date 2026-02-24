using CommerceHub.Application.Services;
using CommerceHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CommerceHub.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service)
    {
        _service = service;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        try
        {
            var order = await _service.CheckoutAsync(request.ProductId, request.Quantity);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var order = await _service.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Order order)
    {
        try
        {
            await _service.UpdateAsync(id, order);
            return Ok("Order updated.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CheckoutRequest(string ProductId, int Quantity);