using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.DTOs;

public class CreateSupplierDto
{
    public string Name{get;set;}
    public string Email{get;set;}
    public string Phone{get;set;}
}

public class SupplierDto
{
    public int Id{get;set;}

    public string Name{get;set;}
    public string Email{get;set;}
    public string Phone{get;set;}
}
