using Application.Configuration;
using Application.Responses;
using Application.Repositories;
using Domain.Entities;
using System.Text.Json;
using System.Security.Cryptography;

namespace Application.Services;

public class JwtService : IJwtService
{
    private readonly IJwtAuthRepository _jwtAuthRepository;

    public JwtService(IJwtAuthRepository jwtAuthRepository)
    {
        _jwtAuthRepository = jwtAuthRepository;
    }

    public string GenerateJwtToken(string audience, Dictionary<string, string> claims, int expirationSeconds)
    {
        return _jwtAuthRepository.GenerateJwtToken(audience, claims, expirationSeconds);
    }

    public Dictionary<string, string> GetJwtClaims(string token)
    {
        return _jwtAuthRepository.GetJwtClaims(token);
    }

    public IEnumerable<object> GetIssuerJwks()
    {
        return _jwtAuthRepository.GetIssuerJwks();
    }
    
    public IEnumerable<string> GetIssuerPublicKeys()
    {
        return _jwtAuthRepository.GetIssuerPublicKeys();
    }
}