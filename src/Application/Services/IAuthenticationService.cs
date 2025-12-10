public interface IAuthenticationService
{
    bool Register(string username, string password);
    bool ValidateUser(string username, string password);
    string GenerateToken(string username);
}