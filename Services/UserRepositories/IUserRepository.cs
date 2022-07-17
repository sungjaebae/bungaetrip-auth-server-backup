using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUserName(string userName);
        Task<User> Create(User user);
        Task<User> GetById(int id);
    }
}
