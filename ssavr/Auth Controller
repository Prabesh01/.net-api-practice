using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{

    // UserManager is responsible for managing users in the system.
    // It handles:
    // - Creating users
    // - Deleting users
    // - Finding users
    // - Updating user info
    // - Assigning roles
    // - Managing passwords (hashing, reset, etc.)
    private readonly UserManager<IdentityUser> _userManager;


    // SignInManager is responsible for authentication (login process).
    // It handles:
    // - Checking username/email + password
    // - Signing users in and out
    // - Managing login sessions (cookies in MVC apps)
    // - Security features like lockout and 2FA
    private readonly SignInManager<IdentityUser> _signInManager;
    /* Why constructor injection is used
    public AuthController(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager)

    ASP.NET Core uses Dependency Injection (DI).
    
    So:
    - You don’t create objects manually
    - Framework gives them automatically
    */
    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // POST api/auth/register-customer
    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDto model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
            return Ok("Customer registered successfully.");
        }
        return BadRequest(result.Errors);
    }

    // POST api/auth/register-admin
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok("Admin registered successfully.");
        }
        return BadRequest(result.Errors);
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded) return Ok("Login successful.");
        return Unauthorized("Invalid login attempt.");
    }
}

