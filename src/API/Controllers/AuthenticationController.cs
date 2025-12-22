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
    public IActionResult Signup([FromBody] SignupRequest request)
    {
        TokenResponse? tokenResponse = _authenticationService.SignUp(request.Username, request.Email, request.FullName, request.Password);
        if (tokenResponse == null)
        {
            return Conflict(
                Problem(
                    detail: "Credentials conflict.",
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflict"
                )
            );
        }
        
        return Ok(tokenResponse);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        TokenResponse? tokenResponse = _authenticationService.Login(request.Username, request.Password);
        if (tokenResponse == null)
        {
            return Unauthorized(
                Problem(
                    detail: "Invalid credentials.",
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Unauthorized"
                )
            );
        }
            
        return Ok(tokenResponse);
    }
}



