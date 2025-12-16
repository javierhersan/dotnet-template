using Application.Responses;
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
        {
            return Ok(_authenticationService.GenerateToken(request.Username)); 
        }
        return Conflict(
            Problem(
                detail: "Username already exists.",
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict"
            )
        );
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        if (_authenticationService.ValidateUser(request.Username, request.Password))
        {
            return Ok(_authenticationService.GenerateToken(request.Username));
        }
        return Unauthorized(
            Problem(
                detail: "Invalid credentials.",
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized"
            )
        );
    }
}



