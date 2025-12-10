using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("signup")]
    public IActionResult Signup([FromBody] AuthRequest request)
    {
        if (_authenticationService.Register(request.Username, request.Password))
            return Ok(new { message = "User registered" });
        return Conflict(new { message = "User already exists" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        if (_authenticationService.ValidateUser(request.Username, request.Password))
        {
            var token = _authenticationService.GenerateToken(request.Username);
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

