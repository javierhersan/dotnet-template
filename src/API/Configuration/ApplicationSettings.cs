namespace API.Configuration;

public class ApplicationSettings
{
    public AzureAd AzureAd { get; set; } = new AzureAd();
    public string DatabaseSecretArn { get; set; } = string.Empty;
}

public class AzureAd
{
    public string Instance { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}