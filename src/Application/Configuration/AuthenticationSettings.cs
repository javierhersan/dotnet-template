namespace Application.Configuration;

public class AuthenticationSettings
{
    public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.None;
    public JwtBearer JwtBearer { get; set; } = new JwtBearer();
    public AzureAd AzureAd { get; set; } = new AzureAd();
}

public enum AuthenticationType
{
    None,
    AzureAd,
    MsEntraId,
    JwtBearer
}

public class JwtBearer
{
    public string Issuer { get; set; } = string.Empty;
    public string IssuerKey { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

public class AzureAd
{
    public string Instance { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
