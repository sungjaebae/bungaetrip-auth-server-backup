using AuthenticationServer.API.Data;
using AuthenticationServer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.API.Services.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;

        public UserRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<User> Create(User user)
        {
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await db.Users.FirstOrDefaultAsync(x=> x.Email == email);
            
        }

        public async Task<User> GetById(int id)
        {
            return await db.Users.FindAsync(id);
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
