using Application.Configuration;
using Application.Responses;
using Application.Repositories;
using Domain.Entities;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public Result<TokenResponse> Login(string username, string password)
    {
        if (_authenticationRepository.ValidateUser(username, password))
        {
            User? user = _authenticationRepository.GetUserByUsername(username);
            return new Result<TokenResponse>()
            {
                Success = true,
                Data = _authenticationRepository.GenerateToken(user?.Id!)
            };
        }

        return new Result<TokenResponse>()
        {
            Success = false,
            Error = new Error
            {
                Code = ErrorCode.Unauthorized,
                Message = "The provided username or password is incorrect."
            }
        };
    }

    public Result<TokenResponse> SignUp(string username, string email, string fullName, string password)
    {
        User? user = _authenticationRepository.Register(username, email, fullName, password);
        if (user != null)
        {
            return new Result<TokenResponse>()
            {
                Success = true,
                Data = _authenticationRepository.GenerateToken(user.Id!)
            };
        }

        return new Result<TokenResponse>()
        {
            Success = false,
            Error = new Error
            {
                Code = ErrorCode.Conflict,
                Message = "The provided credentials conflict with an existing user."
            }
        };
    }

    public Result<TokenResponse> GenerateToken(string username)
    {
        return new Result<TokenResponse>()
        {
            Success = true,
            Data = _authenticationRepository.GenerateToken(username)
        };
    }
        
}