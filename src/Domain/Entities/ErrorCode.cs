namespace Domain.Entities;

public enum ErrorCode
{
    NotFound = 1,
    Unauthorized = 2,
    Forbidden = 3,
    Conflict = 4,
    InternalError = 6,
    ValidationError = 5,
}