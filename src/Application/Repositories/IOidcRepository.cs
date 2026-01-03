using Application.Requests;
using Application.Responses;

namespace Application.Repositories;

public interface IOidcRepository
{
    TokenResponse ExchangeToken(TokenRequest request);
    TokenResponse GenerateExchangeToken(string userId, string clientId = "");
    string GenerateIdToken(string userId);
}