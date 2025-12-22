using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        User? CreateUser(User user);
        User? GetUserById(string id);
        User? GetUserByUsername(string username);
        User? GetUserByEmail(string email);
        User? RemoveUser(string id);
    }
}