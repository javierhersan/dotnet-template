using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Application.Repositories;
using Application.Responses;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly Dictionary<string, string> _users = new();
    private readonly IOAuthRepository _oAuthRepository;

    public AuthenticationRepository(IOAuthRepository oAuthRepository)
    {
        // TODO: Replace OAuth with OIDC
        _oAuthRepository = oAuthRepository;
    }

    public bool Register(string username, string password)
        => _users.TryAdd(username, password);

    public bool ValidateUser(string username, string password)
        => _users.TryGetValue(username, out var stored) && stored == password;

    public TokenResponse GenerateToken(string username)
    {
        return _oAuthRepository.GenerateExchangeToken(username);
    }

}
