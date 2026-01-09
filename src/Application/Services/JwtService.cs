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

    public Result<string> GenerateJwtToken(string audience, Dictionary<string, string> claims, int expirationSeconds)
    {
        return new Result<string>()
        {
            Success = true,
            Data = _jwtAuthRepository.GenerateJwtToken(audience, claims, expirationSeconds)
        };
    }

    public Result<Dictionary<string, string>> GetJwtClaims(string token)
    {
        return new Result<Dictionary<string, string>>()
        {
            Success = true,
            Data = _jwtAuthRepository.GetJwtClaims(token)
        };
    }

    public Result<IEnumerable<object>> GetIssuerJwks()
    {
        return new Result<IEnumerable<object>>()
        {
            Success = true,
            Data = _jwtAuthRepository.GetIssuerJwks()
        };
    }

    public Result<IEnumerable<string>> GetIssuerPublicKeys()
    {
        return new Result<IEnumerable<string>>()
        {
            Success = true,
            Data = _jwtAuthRepository.GetIssuerPublicKeys()
        };
    }
}