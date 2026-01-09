using Application.Responses;
using Domain.Entities;

public interface IAuthenticationService
{
    Result<TokenResponse> Login(string username, string password);  
    Result<TokenResponse> SignUp(string username, string email, string fullName, string password);
    Result<TokenResponse> GenerateToken(string username);
}