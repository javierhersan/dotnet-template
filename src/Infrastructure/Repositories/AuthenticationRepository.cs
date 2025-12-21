using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Application.Repositories;
using Application.Responses;
using Domain.Entities;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly IUserRepository _userRepository;
    private readonly IOAuthRepository _oAuthRepository;
    

    public AuthenticationRepository(IUserRepository userRepository, IOAuthRepository oAuthRepository)
    {
        _userRepository = userRepository;
        _oAuthRepository = oAuthRepository;
    }

    public bool Register(string username, string password)
    {
        User user = new User(username, $"{username}@example.com");
        user.AddPassword(password);
        return _userRepository.CreateUser(user);
    }

    public bool ValidateUser(string username, string password)
    {
        User? user = _userRepository.GetUserByUsername(username);
        if (user == null)
            return false;
        return user.ValidatePassword(password);
    }

    public TokenResponse GenerateToken(string username)
    {
        return _oAuthRepository.GenerateExchangeToken(username);
    }

}
