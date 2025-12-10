using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Configuration;
using Application.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public IActionResult Signup([FromBody] AuthRequest request)
    {
        if (_authService.Register(request.Username, request.Password))
            return Ok(new { message = "User registered" });
        return Conflict(new { message = "User already exists" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        if (_authService.ValidateUser(request.Username, request.Password))
        {
            var token = _authService.GenerateToken(request.Username);
            return Ok(new { token });
        }
        return Unauthorized(new { message = "Invalid credentials" });
    }
}

public class AuthRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthService
{
    private readonly Dictionary<string, string> _users = new();
    private readonly AuthenticationSettings _authSettings;

    public AuthService(IOptions<AuthenticationSettings> authOptions)
    {
        _authSettings = authOptions.Value;
    }

    public bool Register(string username, string password)
        => _users.TryAdd(username, password);

    public bool ValidateUser(string username, string password)
        => _users.TryGetValue(username, out var stored) && stored == password;

    public string GenerateToken(string username)
    {
        var jwtSettings = _authSettings.JwtBearer;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("sub", username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}