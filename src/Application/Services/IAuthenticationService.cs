using Application.Responses;

public interface IAuthenticationService
{
    bool Register(string username, string password);
    bool ValidateUser(string username, string password);
    TokenResponse GenerateToken(string username);
}