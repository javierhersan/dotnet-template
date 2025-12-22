using Application.Responses;
using Domain.Entities;

public interface IAuthenticationService
{
    TokenResponse? Login(string username, string password);  
    TokenResponse? SignUp(string username, string email, string fullName, string password);
    TokenResponse GenerateToken(string username);
}