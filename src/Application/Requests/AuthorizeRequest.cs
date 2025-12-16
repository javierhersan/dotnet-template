namespace Application.Requests;

public class AuthorizeRequest
{
    public string ClientId { get; set; } = default!;
    public string Scope { get; set; } = default!;
    public string RedirectUri { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string ResponseType { get; set; } = "code";
    public string State { get; set; } = default!;
}