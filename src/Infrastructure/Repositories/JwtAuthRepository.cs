using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Application.Repositories;

public class JwtAuthRepository : IJwtAuthRepository
{
    private readonly JwtBearer jwtSettings;

    public JwtAuthRepository(AuthenticationSettings authenticationSettings)
    {
        jwtSettings = authenticationSettings.JwtBearer;
    }

    public string GenerateJwtToken(string issuer, string audience, Dictionary<string, string> claims, int expirationSeconds)
    {
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims.Select(c => new Claim(c.Key, c.Value)),
            expires: DateTime.UtcNow.AddSeconds(expirationSeconds),
            signingCredentials: GetIssuerSigningCredentials());

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Dictionary<string, string> GenerateBaseClaims(string username, string clientId = "")
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("sub", username)
        };

        if (!string.IsNullOrEmpty(clientId))
        {
            claims.Add(new Claim("client_id", clientId));
        }

        return claims.ToDictionary(c => c.Type, c => c.Value);
    }

    public Dictionary<string, string> GetJwtClaims(string token)
    {
        try
        {
            ClaimsPrincipal principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, GetJwtValidationParameters(), out SecurityToken validatedToken);
            return principal.Claims.ToDictionary(c => c.Type, c => c.Value);
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }

    private SigningCredentials GetIssuerSigningCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private TokenValidationParameters GetJwtValidationParameters()
    {
        return new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerKey)),
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };
    }
}
