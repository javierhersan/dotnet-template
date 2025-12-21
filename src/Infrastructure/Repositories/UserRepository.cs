using Application.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;
        public UserRepository()
        {
            _users = new List<User>();
        }

        public bool CreateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Id) || UserExists(user.Id))
            {
                user.Id = GenerateUserId();
            }
            _users.Add(user);
            return true;
        }

        public User? GetUserById(string id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public User? GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public bool RemoveUser(string id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                _users.Remove(user);
                return true;
            }
            return false;
        }

        private string GenerateUserId()
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString();
            } while (UserExists(id));
            return id;
        }

        private bool UserExists(string? id)
        {
            return !string.IsNullOrEmpty(id) && _users.Any(u => u.Id == id);
        }
    }
}