using Application.Configuration;
using Application.Responses;
using Application.Repositories;
using Domain.Entities;

namespace Application.Services;

public interface IJwtService
{
    string GenerateJwtToken(string audience, Dictionary<string, string> claims, int expirationSeconds);
    Dictionary<string, string> GetJwtClaims(string token);
    IEnumerable<object> GetIssuerJwks();
    IEnumerable<string> GetIssuerPublicKeys();
}