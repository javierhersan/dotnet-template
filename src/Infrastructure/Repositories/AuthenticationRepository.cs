using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Application.Repositories;
using Application.DTOs;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly int ACCESS_TOKEN_EXPIRATION_SECONDS = 3600; 
    private readonly Dictionary<string, string> _users = new();
    private readonly AuthenticationSettings _authSettings;

    public AuthenticationRepository(AuthenticationSettings authenticationSettings)
    {
        _authSettings = authenticationSettings;
    }

    public bool Register(string username, string password)
        => _users.TryAdd(username, password);

    public bool ValidateUser(string username, string password)
        => _users.TryGetValue(username, out var stored) && stored == password;

    public TokenResponse GenerateToken(string username)
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
            expires: DateTime.UtcNow.AddSeconds(ACCESS_TOKEN_EXPIRATION_SECONDS),
            signingCredentials: creds);

        return new TokenResponse
        {
            token_type = "Bearer",
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            expires_in = ACCESS_TOKEN_EXPIRATION_SECONDS,
        };
    }
}