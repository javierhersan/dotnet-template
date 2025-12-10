namespace Application.Configuration;

public class ApplicationSettings
{
    public string ServerUrl { get; set; } = string.Empty;
    public string McpServerUrl { get; set; } = string.Empty;
    public string DatabaseSecretArn { get; set; } = string.Empty;
}