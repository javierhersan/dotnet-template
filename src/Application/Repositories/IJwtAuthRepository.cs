namespace Application.Repositories;

public interface IJwtAuthRepository
{
    string GenerateJwtToken(string audience, Dictionary<string, string> claims, int expirationSeconds);
    Dictionary<string, string> GetJwtClaims(string token);
}