using Application.Configuration;
using Application.Responses;
using Application.Repositories;
using Domain.Entities;

namespace Application.Services;

public interface IJwtService
{
    Result<string> GenerateJwtToken(string audience, Dictionary<string, string> claims, int expirationSeconds);
    Result<Dictionary<string, string>> GetJwtClaims(string token);
    Result<IEnumerable<object>> GetIssuerJwks();
    Result<IEnumerable<string>> GetIssuerPublicKeys();
}