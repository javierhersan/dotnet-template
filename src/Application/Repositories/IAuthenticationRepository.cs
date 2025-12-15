using Application.DTOs;

namespace Application.Repositories;

public interface IAuthenticationRepository
{
    bool Register(string username, string password);
    bool ValidateUser(string username, string password);
    TokenResponse GenerateToken(string username);
}