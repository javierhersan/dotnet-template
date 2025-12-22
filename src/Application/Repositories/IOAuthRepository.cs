using Application.Requests;
using Application.Responses;
using Domain.Entities;


namespace Application.Repositories;

public interface IOAuthRepository
{
    OAuthClient RegisterClient(string applicationName, string callbackUrl);
    bool ClientExists(string clientId);
    AuthorizeResponse AuthorizeClient(AuthorizeRequest request, string userId);
    TokenResponse ExchangeToken(TokenRequest request);
    TokenResponse GenerateExchangeToken(string userId, string clientId = "");
    bool ValidateRefreshToken(string userId, string refreshToken);
    void RevokeRefreshToken(string userId);
    public Dictionary<string, string> GenerateOAuthBaseClaims(string userId, string clientId = "");
}