namespace Domain.Entities;


public class OAuthClient
{
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string ApplicationName { get; set; } = default!;
    public string CallbackUrl { get; set; } = default!; // Redirect URI
}