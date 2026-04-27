using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtOptions = jwtOptions.Value;
    }

    // POST api/auth/register-customer
    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDto model)
    {
        return await RegisterByRole(model, "Customer");
    }

    // POST api/auth/register-admin
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto model)
    {
        return await RegisterByRole(model, "Admin");
    }

    // POST api/auth/register-vendor
    [HttpPost("register-vendor")]
    public async Task<IActionResult> RegisterVendor([FromBody] RegisterDto model)
    {
        return await RegisterByRole(model, "Vendor");
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Invalid login attempt.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid login attempt.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var expiryDateUtc = DateTime.UtcNow.AddHours(_jwtOptions.ExpiryHours);
        var token = GenerateJwtToken(user, roles, expiryDateUtc);

        return Ok(new
        {
            token,
            tokenType = "Bearer",
            expiresAtUtc = expiryDateUtc,
            user = new
            {
                id = user.Id,
                email = user.Email,
                roles
            }
        });
    }

private async Task<IActionResult> RegisterByRole(RegisterDto model, string role)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return Conflict(new { message = "User with this email already exists." });
        }

        // Start transaction
        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            //Create user
            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                await tx.RollbackAsync(); // rollback everything
                return BadRequest(new
                {
                    message = "Registration failed.",
                    errors = createResult.Errors.Select(e => e.Description)
                });
            }

            // Assign role
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                await tx.RollbackAsync(); // rollback user creation also
                return BadRequest(new
                {
                    message = "Role assignment failed.",
                    errors = roleResult.Errors.Select(e => e.Description)
                });
            }

            //  Commit (final save)
            await tx.CommitAsync();

            return Ok(new
            {
                message = $"{role} registered successfully.",
                userId = user.Id,
                email = user.Email,
                role
            });
        }
        catch (Exception ex)
        {
            // If any unexpected error happens
            await tx.RollbackAsync();

            return StatusCode(500, new
            {
                message = "Something went wrong.",
                error = ex.Message
            });
        }
    }

    private string GenerateJwtToken(IdentityUser user, IEnumerable<string> roles, DateTime expiryDateUtc)
    {
        // Create user data (claims) that will be stored inside the token
        // Think: "What information do we want to carry in the token?"
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id), // User unique ID
        new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), // User email (JWT standard field)
        new(ClaimTypes.Email, user.Email ?? string.Empty), // Email for ASP.NET use
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
        new(ClaimTypes.NameIdentifier, user.Id), // User ID again (identity system use)
        new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id) // Display name
    };

        // Add user roles into token (Admin, Customer, etc.)
        // This helps in authorization (who can access what)
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        //Create secret key (used to sign token)
        // This ensures token cannot be modified by anyone
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Secret)
        );

        //Define signing method (how token is secured)
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        // Create the JWT token
        // This combines: Header + Claims (data) + Signature
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,        // Who created token
            audience: _jwtOptions.Audience,    // Who can use token
            claims: claims,                    // User data inside token
            expires: expiryDateUtc,           // Token expiry time
            signingCredentials: credentials   // Security signature
        );

        //  Convert token object into string (final token sent to frontend)
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }

        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return Ok(new
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            email = User.FindFirstValue(ClaimTypes.Email),
            roles
        });
    }
}

