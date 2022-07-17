using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.UserRepositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> users = new List<User>();
        public Task<User> Create(User user)
        {
            user.Id = users.Count + 1;
            users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> GetByEmail(string email)
        {
            return Task.FromResult(users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User> GetById(int id)
        {
            return Task.FromResult(users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User> GetByUserName(string userName)
        {
            return Task.FromResult(users.FirstOrDefault(u => u.UserName == userName));
        }
    }
}
