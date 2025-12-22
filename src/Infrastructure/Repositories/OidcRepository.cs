// Same steps as OAuth2.0 but with additional OIDC-specific Scopes (Scope:OPENID)
// and ID token response with user claims (sub, name, email, etc.)

// Adds also OPEID Discovery endpoint: https://your-auth-server/.well-known/openid-configuration
// Adds also UserInfo endpoint to get more user claims (beyond ID token): https://your-auth-server/userinfo
// UserInfo endpoint is called by the client application after getting the ID token and access token.

// OAuth Access Token and Refresh Token generation should not contain OIDC-specific claims (like 'sub', 'name', 'email') to not reveal sensitive user information.
// Sub can be exposed (User ID) but avoid adding more claims in access token for better security and privacy.

using Application.Configuration;
using Application.Repositories;
using Application.Responses;
using Application.Requests;
using Domain.Entities;

// Identity Provider (OIDC) 
public class OidcRepository : IOidcRepository
{
    private readonly int OIDC_TOKEN_EXPIRATION_SECONDS = 3600; // 1 hour
    private readonly JwtBearer _jwtSettings;
    private readonly IUserRepository _userRepository;

    private readonly IOAuthRepository _oAuthRepository;
    private readonly IJwtAuthRepository _jwtAuthRepository;

    public OidcRepository(AuthenticationSettings authSettings, IOAuthRepository oAuthRepository, IJwtAuthRepository jwtAuthRepository, IUserRepository userRepository)
    {
        _jwtSettings = authSettings.JwtBearer;
        _oAuthRepository = oAuthRepository;
        _jwtAuthRepository = jwtAuthRepository;
        _userRepository = userRepository;
    }

    public TokenResponse ExchangeToken(TokenRequest request)
    {
        TokenResponse oAuthToken = _oAuthRepository.ExchangeToken(request);
        string? userId = _jwtAuthRepository.GetJwtClaims(request.Code).Where(c => c.Key == "sub").Select(c => c.Value).FirstOrDefault();

        oAuthToken.id_token = GenerateIdToken(userId!);
        return oAuthToken;

    }

    public TokenResponse GenerateExchangeToken(string userId, string clientId = "")
    {
        TokenResponse oAuthToken = _oAuthRepository.GenerateExchangeToken(userId, clientId);
        oAuthToken.id_token = GenerateIdToken(userId);
        return oAuthToken;
    }

    public string GenerateIdToken(string userId)
    {
        return _jwtAuthRepository.GenerateJwtToken(
                _jwtSettings.Issuer, 
                _jwtSettings.Audience, 
                claims: GenerateOpenIdBaseClaims(userId), 
                OIDC_TOKEN_EXPIRATION_SECONDS
            );
    }

    private Dictionary<string, string> GenerateOpenIdBaseClaims(string userId)
    {
        User? user = _userRepository.GetUserById(userId);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        var claims = new Dictionary<string, string>
        {
            { "sub", user.Id!.ToString() },
            { "name", user.FullName },
            { "email", user.Email }
        };

        return claims;
    }
}