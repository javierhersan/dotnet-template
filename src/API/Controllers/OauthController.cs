using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("oauth2/v2.0")]
public class OauthController : ControllerBase
{
    public OauthController()
    {
        
    }

    // Additional Endpoints for Oauth2.0
    // token_endpoint_auth_methods_supported = new[] { "client_secret_post", "private_key_jwt" },
    // jwks_uri = "https://your-auth-server/discovery/v2.0/keys",

    // References:
    // https://learn.microsoft.com/en-us/entra/identity-platform/authentication-flows-app-scenarios
    // https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc
    // https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        return Ok();
    }

    [HttpGet("token")]
    public IActionResult Token()
    {
        return Ok();
    }
    
}