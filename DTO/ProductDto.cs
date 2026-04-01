using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.DTOs;

public class CreateProductDto
{
    [Required]
    public string Name {get;set;}

    [Required]
    public string SKU{get;set;}

    [Required(ErrorMessage = "Price is required.")]
    [Range(1, double.MaxValue, ErrorMessage = "Price must be at least 1.")]
    public decimal Price {get;set;}

    public int Stock{get;set;}

    [Required]
    public int SupplierId{get;set;}

    public string? Description {get;set;}

    [Required]
    public int CategoryId{get;set;}

    public string ImageUrl {get;set;}
}

public class UpdateProductDto
{
    [Required]
    public int Id{get;set;}

    [Required]
    public string Name {get;set;}

    [Required]
    public string SKU{get;set;}

    [Required(ErrorMessage = "Price is required.")]
    [Range(1, double.MaxValue, ErrorMessage = "Price must be at least 1.")]
    public decimal Price {get;set;}

    public int? Stock{get;set;}

    [Required]
    public int SupplierId{get;set;}

    public string? Description {get;set;}

    [Required]
    public int CategoryId{get;set;}

    public string ImageUrl {get;set;}
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU{get;set;}
    public decimal Price {get;set;}
    public int Stock{get;set;}
    public int SupplierId{get;set;}

    public string? Description {get;set;}
    public int CategoryId{get;set;}

    public string ImageUrl {get;set;}
}
