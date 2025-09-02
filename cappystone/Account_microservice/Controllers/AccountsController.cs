using Account_microservice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Account_microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Hardcoded JWT values (must match your JWTExtension.cs)
        private const string JwtIssuer = "cwms-app-issuer";
        private const string JwtAudience = "cwms-app-audience";
        private const string JwtKey = "cwmsapp_9kQ2!r8F7zT4eB1pV6mN0yX3aC5dG"; // 256-bit length recommended
        private const int JwtExpiryMinutes = 120;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // POST: /api/account/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null) return Conflict("Email already registered.");

            // Use your custom user type
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName   // optional: if your DTO & ApplicationUser have this
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign role based on some condition (e.g. dto.Role or email domain)
            string role = dto.Email.EndsWith("@admin.com") ? "Admin" : "Worker";

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            var roles = await _userManager.GetRolesAsync(user);
            var response = CreateJwtResponse(user, roles);

            return Ok(response);
        }


        // POST: /api/account/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!ok) return Unauthorized("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var response = CreateJwtResponse(user, roles);

            return Ok(response);
        }

        // Example protected endpoint (optional sanity check)
        [Authorize(Roles = "Worker")]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                email = User.FindFirstValue(ClaimTypes.Email),
                roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray()
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName
            });
        }

        // --- helpers ---
        private AuthResponse CreateJwtResponse(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id),
        new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email ?? string.Empty),
        // You can also add custom claims from ApplicationUser here if needed
        new("full_name", user.FullName ?? string.Empty)
    };
            foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(JwtExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new AuthResponse(
                Token: new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt: expires,
                UserId: user.Id,
                Email: user.Email ?? string.Empty,
                Roles: roles.ToArray()
            );
        }


        // --- DTOs (inline for convenience) ---
        public record RegisterDto(string Email, string Password, string FullName);
        public record LoginDto(string Email, string Password);
        public record AuthResponse(string Token, DateTime ExpiresAt, string UserId, string Email, string[] Roles);
    
}
}
