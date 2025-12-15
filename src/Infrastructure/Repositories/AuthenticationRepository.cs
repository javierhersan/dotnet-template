using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Application.Repositories;
using Application.DTOs;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly int ACCESS_TOKEN_EXPIRATION_SECONDS = 3600; // 1 hour
    private readonly int REFRESH_TOKEN_EXPIRATION_SECONDS = 86400; // 1 day
    private readonly Dictionary<string, string> _users = new();
    private readonly Dictionary<string, string> _refreshTokens = new();
    private readonly JwtBearer jwtSettings;

    public AuthenticationRepository(AuthenticationSettings authenticationSettings)
    {
        jwtSettings = authenticationSettings.JwtBearer;
    }

    public bool Register(string username, string password)
        => _users.TryAdd(username, password);

    public bool ValidateUser(string username, string password)
        => _users.TryGetValue(username, out var stored) && stored == password;

    public TokenResponse GenerateToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("sub", username)
        };

        string accessToken = GenerateAccessToken(jwtSettings.Issuer, jwtSettings.Audience, claims, creds, ACCESS_TOKEN_EXPIRATION_SECONDS);
        string refreshToken = GenerateRefreshToken();
        SaveUserRefreshToken(username, refreshToken);

        return new TokenResponse
        {
            token_type = "Bearer",
            access_token = accessToken,
            expires_in = ACCESS_TOKEN_EXPIRATION_SECONDS,
            ext_expires_in = ACCESS_TOKEN_EXPIRATION_SECONDS,
            refresh_token = refreshToken,
            refresh_token_expires_in = REFRESH_TOKEN_EXPIRATION_SECONDS,
            id_token = string.Empty,
            client_info = string.Empty,
        };
    }

    private string GenerateAccessToken(string issuer, string audience, IEnumerable<Claim> claims, SigningCredentials creds, int expirationSeconds)
    {
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expirationSeconds),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
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
            
            ClaimsPrincipal principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    private void SaveUserRefreshToken(string username, string refreshToken)
    {
        _refreshTokens[username] = refreshToken;
    }

    public bool ValidateRefreshToken(string username, string refreshToken)
    {
        return _refreshTokens.TryGetValue(username, out var storedToken) && storedToken == refreshToken;
    }

    public void RevokeRefreshToken(string username)
    {
        _refreshTokens.Remove(username);
    }
}