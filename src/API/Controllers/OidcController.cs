using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route(".well-known")]
public class OidcController : ControllerBase
{
    public OidcController()
    {
        
    }

    // References:
    // https://learn.microsoft.com/en-us/entra/identity-platform/authentication-flows-app-scenarios
    // https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc
    // https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols

    [HttpGet("openid-configuration")]
    public IActionResult GetOpenIdConfiguration()
    {
        var config = new
        {
            authorization_endpoint = "https://your-auth-server/oauth2/v2.0/authorize",
            token_endpoint = "https://your-auth-server/oauth2/v2.0/token",
            token_endpoint_auth_methods_supported = new[] { "client_secret_post", "private_key_jwt" },
            jwks_uri = "https://your-auth-server/discovery/v2.0/keys",
            userinfo_endpoint = "https://your-auth-server/userinfo",
            subject_types_supported = new[] { "pairwise" }
        };
        return Ok(config);
    }
    
}