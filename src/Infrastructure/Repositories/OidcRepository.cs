// Same steps as OAuth2.0 but with additional OIDC-specific Scopes (Scope:OPENID)
// and ID token response with user claims (sub, name, email, etc.)

// Adds also OPEID Discovery endpoint: https://your-auth-server/.well-known/openid-configuration
// Adds also UserInfo endpoint to get more user claims (beyond ID token): https://your-auth-server/userinfo
// UserInfo endpoint is called by the client application after getting the ID token and access token.
