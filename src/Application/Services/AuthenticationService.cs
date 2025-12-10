using Application.Configuration;
using Application.Repositories;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public bool Register(string username, string password)
        => _authenticationRepository.Register(username, password);

    public bool ValidateUser(string username, string password)
        => _authenticationRepository.ValidateUser(username, password);

    public string GenerateToken(string username)
        => _authenticationRepository.GenerateToken(username);
}