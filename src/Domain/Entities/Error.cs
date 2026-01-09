namespace Domain.Entities;

public class Error
{
    public ErrorCode Code { get; set; }
    public string Message { get; set; } = string.Empty;
}