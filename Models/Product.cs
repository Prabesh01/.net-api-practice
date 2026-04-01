public class Product
{
    public int Id{get;set;}
    
    public string Name {get;set;}

    public string SKU{get;set;}
    public decimal Price {get;set;}
    public int Stock{get;set;}
    public int SupplierId{get;set;}
    public Supplier Supplier{get;set;}

    public string? Description {get;set;}
    public int CategoryId{get;set;}

    public Category Category {get;set;}
    public string ImageUrl {get;set;}
    // public ICollection<OrderItem> OrderItems{get;set;}

}
