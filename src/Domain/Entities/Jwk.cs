namespace Domain.Entities;


public class Jwk
{
    public required string kty { get; set; }
    public required string n { get; set; }
    public required string e { get; set; }
    public required string alg { get; set; }
    public required string use { get; set; }
    public required string kid { get; set; }
}