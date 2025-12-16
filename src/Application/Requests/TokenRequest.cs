namespace Application.Requests;

public class TokenRequest 
{
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string GrantType { get; set; } = "authorization_code";
    public string Code { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}