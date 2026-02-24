using CommerceHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceHub.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepo;

    public ProductsController(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> AdjustStock(string id, [FromBody] StockRequest request)
    {
        var success = await _productRepo.AdjustStockAsync(id, request.Quantity);

        if (!success)
            return BadRequest(new { message = "Stock update failed." });

        return Ok("Stock updated.");
    }
}

public record StockRequest(int Quantity);