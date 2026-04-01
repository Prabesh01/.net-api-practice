using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Select(c => new ProductDto { Id = c.Id, Name = c.Name, SKU = c.SKU, Description = c.Description, Price = c.Price, Stock = c.Stock, SupplierId = c.SupplierId, CategoryId = c.CategoryId, ImageUrl = c.ImageUrl })
            .ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = new Product { Name = dto.Name, SKU = dto.SKU, Description = dto.Description, Price = dto.Price, Stock = dto.Stock, SupplierId = dto.SupplierId, CategoryId = dto.CategoryId, ImageUrl = dto.ImageUrl  };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }

    [HttpGet("count")]
    public async Task<IActionResult> Count()
        => Ok(new { totalProducts = await _context.Products.CountAsync() });
}
