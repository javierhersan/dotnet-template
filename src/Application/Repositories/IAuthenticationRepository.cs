using Application.Responses;
using Domain.Entities;

namespace Application.Repositories;

public interface IAuthenticationRepository
{
    User? GetUserByUsername(string username);
    User? Register(string username, string email, string fullName, string password);
    bool ValidateUser(string username, string password);
    TokenResponse GenerateToken(string userId, bool useOidc = true);
}