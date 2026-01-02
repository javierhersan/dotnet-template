using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Application.Repositories;
using System.Security.Cryptography;

public class JwtAuthRepository : IJwtAuthRepository
{
    private readonly List<RSA> jwtPrivateKeys;
    private readonly JwtBearer jwtSettings;

    public JwtAuthRepository(AuthenticationSettings authenticationSettings)
    {
        jwtPrivateKeys = new List<RSA>
        {
            RSA.Create(2048), 
        };
        jwtSettings = authenticationSettings.JwtBearer;
    }

    public string GenerateJwtToken(string audience, Dictionary<string, string> claims, int expirationSeconds)
    {
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: audience,
            claims: claims.Select(c => new Claim(c.Key, c.Value)),
            expires: DateTime.UtcNow.AddSeconds(expirationSeconds),
            signingCredentials: GetIssuerSigningCredentials()
        );
            

        return new JwtSecurityTokenHandler().WriteToken(token);
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
                    // NameClaimType = "name",
                    // RoleClaimType = "roles"
                };
    }

    public IEnumerable<object> GetIssuerJwks()
    {
        return jwtPrivateKeys.Select((rsa, idx) =>
        {
            var parameters = rsa.ExportParameters(false);
            var jwk = new
            {
                kty = "RSA",
                n = Base64UrlEncode(parameters.Modulus!),
                e = Base64UrlEncode(parameters.Exponent!),
                alg = "RS256",
                use = "sig",
                kid = idx.ToString()
            };
            return jwk;
        });
    }
    
    public IEnumerable<string> GetIssuerPublicKeys()
    {
        return jwtPrivateKeys.Select((rsa, idx) =>
        {
            var parameters = rsa.ExportParameters(false);
            var jwk = new
            {
                kty = "RSA",
                n = Base64UrlEncode(parameters.Modulus!),
                e = Base64UrlEncode(parameters.Exponent!),
                alg = "RS256",
                use = "sig",
                kid = idx.ToString()
            };
            return System.Text.Json.JsonSerializer.Serialize(jwk);
        });
    }

    private SigningCredentials GetIssuerSigningCredentials()
    {
        var rsa = jwtPrivateKeys[0];
        var key = new RsaSecurityKey(rsa)
        {
            KeyId = "0" //Guid.NewGuid().ToString()
        };
        return new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
    }

    public string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public byte[] Base64UrlDecode(string input)
    {
        string padded = input.Length % 4 == 0
            ? input
            : input + new string('=', 4 - input.Length % 4);
        string base64 = padded.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64);
    }
}
