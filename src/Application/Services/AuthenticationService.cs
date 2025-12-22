using Application.Configuration;
using Application.Responses;
using Application.Repositories;
using Domain.Entities;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public TokenResponse? Login(string username, string password)
    {
        if (_authenticationRepository.ValidateUser(username, password))
        {
            User? user = _authenticationRepository.GetUserByUsername(username);
            return _authenticationRepository.GenerateToken(user?.Id!);
        }
        return null;
    }

    public TokenResponse? SignUp(string username, string email, string fullName, string password)
    {
        User? user = _authenticationRepository.Register(username, email, fullName, password);
        if (user != null)
        {
            return _authenticationRepository.GenerateToken(user.Id!);
        }
        return null;
    }

    public TokenResponse GenerateToken(string username) 
        => _authenticationRepository.GenerateToken(username);
}