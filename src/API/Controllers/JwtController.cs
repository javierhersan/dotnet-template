using System.Text.Json;
using Application.Responses;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route(".well-known")]
public class JwtController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public JwtController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpGet("jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(new { keys = _jwtService.GetIssuerJwks().Data });
    }
    
}