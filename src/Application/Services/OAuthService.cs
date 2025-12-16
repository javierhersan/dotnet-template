// TODO: OAuth Service and OAuth Controller (WIP)
// TODO: Implement OIDC Service, OIDC Repository and OIDC Controller
// [Route("oauth2/v2.0")]
// public class OAuthService : IOAuthService
// {
//     public OAuthService()
//     {
//     }

//     public Task<Result> Authorize(AuthorizeRequest request)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<Result> Token(TokenRequest request)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<Result> Redirect(RedirectRequest request)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<Result> Challenge(ChallengeRequest request)
//     {
//         throw new NotImplementedException();
//     }

//     public bool ValidateUser(string username, string password)
//     {
//         // Dummy validation
//         return username == "demoUser" && password == "demoPass";
//     }

//     public string GenerateToken(string username)
//     {
//         // Dummy token
//         return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
//     }

//     public bool IsValidClient(string clientId, string redirectUri)
//     {
//         // Dummy validation
//         return clientId == "" &&
//                redirectUri.StartsWith("https://");
//     }

//     public string GenerateAuthorizationCode(string username, string clientId, string redirectUri, string scope)
//     {
//         // Dummy code
//         return Guid.NewGuid().ToString("N");
//     }
// }

// public class Result
// {
//     public bool Success { get; set; } = false;
//     public object? Data { get; set; } = null;
//     public string Error { get; set; } = string.Empty;
// }

// public class AuthorizeRequest
// {
//     public string ClientId { get; set; } = string.Empty;
//     public string Scope { get; set; } = string.Empty;
//     public string RedirectUri { get; set; } = string.Empty;
//     public string ClientRequestId { get; set; } = string.Empty;
//     public string ResponseMode { get; set; } = string.Empty;
//     public string ResponseType { get; set; } = string.Empty;
//     public string XClientSKU { get; set; } = string.Empty;
//     public string XClientVER { get; set; } = string.Empty;
//     public string ClientInfo { get; set; } = string.Empty;
//     public string CodeChallenge { get; set; } = string.Empty;
//     public string CodeChallengeMethod { get; set; } = string.Empty;
//     public string Nonce { get; set; } = string.Empty;
//     public string State { get; set; } = string.Empty;
// }

// public class TokenResponse
// {
//     public string access_token { get; set; } = string.Empty;
//     public string client_info { get; set; } = string.Empty;
//     public int expires_in { get; set; }
//     public int ext_expires_in { get; set; }
//     public string id_token { get; set; } = string.Empty;
//     public string refresh_token { get; set; } = string.Empty;
//     public int refresh_token_expires_in { get; set; }
//     public string scope { get; set; } = string.Empty;
//     public string token_type { get; set; } = string.Empty;
// }

// public class ChallengeRequest
// {
//     public string StateHandle { get; set; } = string.Empty;
//     public Authenticator Authenticator { get; set; } = new Authenticator();
// }

// public class Authenticator
// {
//     public string Id { get; set; } = string.Empty;
//     public string MethodType { get; set; } = string.Empty;
// }

// public class ChallengeResponse
// {
//     public Authenticator authenticator { get; set; } = new Authenticator();
//     public string stateHandle { get; set; } = string.Empty;
// }