using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        bool CreateUser(User user);
        User? GetUserById(string id);
        User? GetUserByUsername(string username);
        User? GetUserByEmail(string email);
        bool RemoveUser(string id);
    }
}