namespace Application.Responses;

public class AuthorizeResponse
{
    public string Code { get; set; } = default!;
    public string State { get; set; } = default!;
}