using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) => _context = context;

    // [HttpGet]
    // public async Task<IActionResult> GetAll()
    // {
    //     var products = await _context.Products
    //         .Select(c => new ProductDto { Id = c.Id, Name = c.Name, SKU = c.SKU, Description = c.Description, Price = c.Price, Stock = c.Stock, SupplierId = c.SupplierId, CategoryId = c.CategoryId, ImageUrl = c.ImageUrl })
    //         .ToListAsync();
    //     return Ok(products);
    // }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto filter)
    {
    var query = _context.Products.AsQueryable();
    if (!string.IsNullOrEmpty(filter.Name))
        query = query.Where(p => p.Name.ToLower()
                        .Contains(filter.Name.ToLower()));
    if (!string.IsNullOrEmpty(filter.SKU))
        query = query.Where(p => p.SKU.ToLower() == filter.SKU.ToLower());
    if (filter.MinPrice.HasValue)
        query = query.Where(p => p.Price >= filter.MinPrice.Value);
    if (filter.MaxPrice.HasValue)
        query = query.Where(p => p.Price <= filter.MaxPrice.Value);
    if (filter.CategoryId.HasValue)
        query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
    var results = await query.ToListAsync();
    return Ok(results);
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
