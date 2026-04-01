using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options; 

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    // private readonly IConfiguration _configuration;
    private readonly PaymentServiceOptions _configuration;

    // public PaymentController(IConfiguration _configuration){
    //     _configuration=_configuration;
    // }

    public PaymentController(IOptions<PaymentServiceOptions> options){
        _configuration=options.Value;
    }

    [HttpGet("url")]
    public IActionResult GetPaymentUrl()
    {
        // string apiUrl=_configuration["PaymentService:ApiUrl"];
        string apiUrl=_configuration.ApiUrl;
        return Ok(new{PaymentUrl=apiUrl});
    }
}
