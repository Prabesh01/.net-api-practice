using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _context;
    public SuppliersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var suppliers = await _context.Suppliers
            .Select(c => new SupplierDto { Id = c.Id, Name = c.Name, Email = c.Email, Phone = c.Phone })
            .ToListAsync();
        return Ok(suppliers);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSupplierDto dto)
    {
        var supplier = new Supplier { Name = dto.Name, Email = dto.Email, Phone = dto.Phone };
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return Ok(supplier);
    }

    [HttpGet("count")]
    public async Task<IActionResult> Count()
        => Ok(new { totalSupplier = await _context.Suppliers.CountAsync() });
}
