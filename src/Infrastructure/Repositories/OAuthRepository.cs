using Application.Configuration;
using Application.Repositories;
using Application.Responses;
using Application.Requests;
using Domain.Entities;

// Authorization Server (OAuth 2.0) and Identity Provider (OIDC) 
public class OAuthRepository : IOAuthRepository
{
    private readonly int AUTHORIZE_TOKEN_EXPIRATION_SECONDS = 300 ; // 5 minutes 
    private readonly int ACCESS_TOKEN_EXPIRATION_SECONDS = 3600; // 1 hour
    private readonly int REFRESH_TOKEN_EXPIRATION_SECONDS = 7 * 86400; // 7 days 
    private readonly Dictionary<string, string> _refreshTokens = new();
    private readonly Dictionary<string, OAuthClient> _oauthClients = new(); 
    private readonly JwtBearer _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly IJwtAuthRepository _jwtAuthRepository;

    public OAuthRepository(AuthenticationSettings authSettings, IUserRepository userRepository, IJwtAuthRepository jwtAuthRepository)
    {
        _jwtSettings = authSettings.JwtBearer;
        _userRepository = userRepository;
        _jwtAuthRepository = jwtAuthRepository;
    }

    /// <summary>
    /// OAuth Step 1 (3rd Party Client Registration)
    /// Registers a new client application and returns its credentials.
    /// </summary>
    /// <param name="applicationName"></param>
    /// <param name="callbackUrl"></param>
    /// <returns></returns>
    public OAuthClient RegisterClient(string applicationName, string callbackUrl)
    {
        OAuthClient oAuthClient = new OAuthClient
        {
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = Guid.NewGuid().ToString(),
            ApplicationName = applicationName,
            CallbackUrl = callbackUrl
        };

        _oauthClients[oAuthClient.ClientId] = oAuthClient;

        return oAuthClient;
    }

    public bool ClientExists(string clientId)
    {
        return _oauthClients.ContainsKey(clientId);
    }

    public bool ValidateClientSecret(string clientId, string clientSecret)
    {
        if (!ClientExists(clientId))
        {
            return false;
        }

        if (_oauthClients[clientId].ClientSecret != clientSecret) 
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// OAuth Step 2 (Authorize Client Application on behalf of User)
    /// Generates an authorization code for the 3rd party client application on behalf of the user.
    /// User is redirected to the Authorization Server Provider and logs in. Then the user authorizes the 3rd party client application. An authorize request is made to generate an authorization code.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public AuthorizeResponse AuthorizeClient(AuthorizeRequest request, string username)
    {
        if (!ClientExists(request.ClientId))
        {
            throw new Exception("Invalid Client ID");
        }

        return new AuthorizeResponse
        {
            Code = _jwtAuthRepository.GenerateJwtToken(
                _jwtSettings.Issuer, 
                _jwtSettings.Audience, 
                _jwtAuthRepository.GenerateBaseClaims(username, request.ClientId), 
                AUTHORIZE_TOKEN_EXPIRATION_SECONDS
            ),
            State = request.State
        };
    }

    /// <summary>
    /// OAuth Step 3 (Token Exchange)
    /// Exchanges the authorization code for an access token.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public TokenResponse ExchangeToken(TokenRequest request)
    {
        if (!ClientExists(request.ClientId))
        {
            throw new Exception("Invalid Client ID");
        }
        
        string? username = _jwtAuthRepository.GetJwtClaims(request.Code).Where(c => c.Key == "sub").Select(c => c.Value).FirstOrDefault();

        if (username == null)
        {
            throw new Exception("Invalid Authorization Code");
        }

        return GenerateExchangeToken(username, request.ClientId);
        
    }

    public TokenResponse GenerateExchangeToken(string username, string clientId = "")
    {
        string accessToken = _jwtAuthRepository.GenerateJwtToken(
                _jwtSettings.Issuer, 
                _jwtSettings.Audience, 
                _jwtAuthRepository.GenerateBaseClaims(username, clientId), 
                ACCESS_TOKEN_EXPIRATION_SECONDS
            );
        
        string refreshToken = _jwtAuthRepository.GenerateJwtToken(
                _jwtSettings.Issuer, 
                _jwtSettings.Audience, 
                _jwtAuthRepository.GenerateBaseClaims(username, clientId), 
                REFRESH_TOKEN_EXPIRATION_SECONDS
            );

        SaveUserRefreshToken(username, refreshToken);

        return new TokenResponse
        {
            token_type = "Bearer",
            access_token = accessToken,
            expires_in = ACCESS_TOKEN_EXPIRATION_SECONDS,
            ext_expires_in = ACCESS_TOKEN_EXPIRATION_SECONDS,
            refresh_token = refreshToken,
            refresh_token_expires_in = REFRESH_TOKEN_EXPIRATION_SECONDS,
            client_info = clientId,
            id_token = string.Empty,
        };
    }

    private void SaveUserRefreshToken(string username, string refreshToken)
    {
        _refreshTokens[username] = refreshToken;
    }

    public bool ValidateRefreshToken(string username, string refreshToken)
    {
        return _refreshTokens.TryGetValue(username, out var storedToken) && storedToken == refreshToken;
    }

    public void RevokeRefreshToken(string username)
    {
        _refreshTokens.Remove(username);
    }
}