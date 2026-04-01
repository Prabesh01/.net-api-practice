using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options; 

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
    private readonly MyInfoOptions _ioption;
    private readonly IConfiguration _iconfig;

    public InfoController(IOptions<MyInfoOptions> options, IConfiguration config){
        _ioption=options.Value;
         _iconfig=config;

    }

    [HttpGet("ioption/")]
    public IActionResult GetMyInfoWithIOption()
    {
        string name=_ioption.Name;
        int age = _ioption.Age;
        string address= _ioption.Address;
        string contact = _ioption.Contact;
        return Ok(new{name=name, age=age, address=address, contact=contact});
    }

    [HttpGet("iconfig/")]
    public IActionResult GetMyInfoWithIConfig()
    {
        string Name=_iconfig["MyInfo:Name"];
        string Age=_iconfig["MyInfo:Age"];
        string Address=_iconfig["MyInfo:Address"];
        string Contact=_iconfig["MyInfo:Contact"];
        return Ok(new{name=Name,age=Age,address=Address,contact=Contact});
    }

}
