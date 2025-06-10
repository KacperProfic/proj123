using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Projekt_zaliczenie.Data;
using Projekt_zaliczenie.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // Dodana biblioteka BCrypt
using System.ComponentModel.DataAnnotations;

namespace Projekt_zaliczenie.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MissionContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(MissionContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == model.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.PasswordHash, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            return BadRequest("Username already exists");
        var user = new User
        {
            Username = model.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully");
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string PasswordHash { get; set; } // Tutaj podajesz plain text hasło
}

public class RegisterModel
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 4)]
    public string Password { get; set; }
}