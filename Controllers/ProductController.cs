using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeatherAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;
    // Cache keys (avoid hardcoded strings everywhere)
    private const string PRODUCTS_ALL_KEY = "products_all";
    public ProductsController(AppDbContext context, IMemoryCache cache) { _context = context; _cache = cache; }

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

        // Check if filtering is applied
        bool hasFilter =
            filter.Name != null || filter.SKU != null ||
            filter.MinPrice != null || filter.MaxPrice != null ||
            filter.CategoryId != null;

        // Cache HIT (only when no filter is applied)
        if (!hasFilter &&
            _cache.TryGetValue(PRODUCTS_ALL_KEY, out List<Product>? cachedProducts))
        {
            return Ok(cachedProducts);
        }

        // Cache MISS → fetch from DB
        var results = await query.ToListAsync();

        // Store in cache only if no filter
        if (!hasFilter)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                // AbsoluteExpiration = cache will be removed after 5 minutes no matter what

                .SetSlidingExpiration(TimeSpan.FromMinutes(2));
            // SlidingExpiration = if not accessed for 2 minutes → removed
            // If accessed frequently → timer resets
            _cache.Set(PRODUCTS_ALL_KEY, results, options);
        }
        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        string cacheKey = $"product_{id}";

        // Try cache first
        if (_cache.TryGetValue(cacheKey, out ProductDto? cachedProduct))
        {
            return Ok(cachedProduct);
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var result = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            CategoryId = product.CategoryId,
            SupplierId = product.SupplierId
        };

        // Store in cache
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = new Product { Name = dto.Name, SKU = dto.SKU, Description = dto.Description, Price = dto.Price, Stock = dto.Stock, SupplierId = dto.SupplierId, CategoryId = dto.CategoryId, ImageUrl = dto.ImageUrl  };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _cache.Remove(PRODUCTS_ALL_KEY);
        return Ok(product);
    }

    [HttpGet("count")]
    public async Task<IActionResult> Count()
        => Ok(new { totalProducts = await _context.Products.CountAsync() });

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.Name = dto.Name;
        product.SKU = dto.SKU;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;
        product.SupplierId = dto.SupplierId;

        await _context.SaveChangesAsync();

        // Clear related cache
        _cache.Remove(PRODUCTS_ALL_KEY);
        _cache.Remove($"product_{id}");

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        // Clear cache
        _cache.Remove(PRODUCTS_ALL_KEY);
        _cache.Remove($"product_{id}");

        return NoContent();
    }
}
