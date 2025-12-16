using Application.Requests;
using Application.Responses;
using Domain.Entities;


namespace Application.Repositories;

public interface IOAuthRepository
{
    OAuthClient RegisterClient(string applicationName, string callbackUrl);
    bool ClientExists(string clientId);
    AuthorizeResponse AuthorizeClient(AuthorizeRequest request, string username);
    TokenResponse ExchangeToken(TokenRequest request);
    TokenResponse GenerateExchangeToken(string username, string clientId = "");
    bool ValidateRefreshToken(string username, string refreshToken);
    void RevokeRefreshToken(string username);
}