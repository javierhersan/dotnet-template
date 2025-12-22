using Application.Repositories;
using Application.Responses;
using Domain.Entities;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly IUserRepository _userRepository;
    private readonly IOAuthRepository _oAuthRepository;
    private readonly IOidcRepository _oidcRepository;

    public AuthenticationRepository(IUserRepository userRepository, IOAuthRepository oAuthRepository, IOidcRepository oidcRepository)
    {
        _userRepository = userRepository;
        _oAuthRepository = oAuthRepository;
        _oidcRepository = oidcRepository;
    }

    public User? GetUserByUsername(string username)
    {
        return _userRepository.GetUserByUsername(username);
    }

    public User? Register(string username, string email, string fullName, string password)
    {
        User user = new User(username, email, fullName);
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

    public TokenResponse GenerateToken(string userId, bool useOidc = true)
    {
        if (useOidc) return _oidcRepository.GenerateExchangeToken(userId);
        return _oAuthRepository.GenerateExchangeToken(userId);
    }

}
