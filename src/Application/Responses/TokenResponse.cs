namespace Application.Responses;

public class TokenResponse
{
    public string token_type { get; set; } = "Bearer";
    public string scope { get; set; } = string.Empty;
    public int expires_in { get; set; } = 3600;
    public int ext_expires_in { get; set; } = 3600;
    public string access_token { get; set; } = string.Empty;
    public string refresh_token { get; set; } = string.Empty;
    public int refresh_token_expires_in { get; set; } = 86400;
    public string id_token { get; set; } = string.Empty;
    public string client_info { get; set; } = string.Empty;
}