using AuthenticationServer.API.Data;
using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Services.PasswordHashers;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationServer.API.Services
{
    public class DbInitializer
    {
        private readonly IPasswordHasher passwordHasher;
        private readonly AuthenticationDbContext db;
        private readonly UserManager<User> userRepository;
        private readonly string[] lastName = { "김", "이", "박", "최", "한", "오", "배" };
        private readonly string[] firstName = { "성재", "형수", "진석", "두훈", "저스틴", "한별" };
        private readonly Random rand = new Random();
        private readonly string[] gender = { "MALE", "FEMALE" };

        public DbInitializer(IPasswordHasher passwordHasher, AuthenticationDbContext db, UserManager<User> userRepository)
        {
            this.passwordHasher = passwordHasher;
            this.db = db;
            this.userRepository = userRepository;
        }

        public async Task init()
        {
            var passwordHash = passwordHasher.HashPassword("1234");
            for (int i = 0; i < 10; i++)
            {
                Member member = new Member()
                {
                    CreatedAt = DateTime.UtcNow,
                    Email = $"seedMail{i}@google.com",
                    UserName = $"seedUsername{i}",
                    Nickname = lastName[rand.Next(0, 5)] + firstName[rand.Next(0, 6)],
                    Password = passwordHash,
                    Gender = gender[rand.Next(0, 2)],
                    Role = "ROLE_USER"
                };
                await db.Members.AddAsync(member);
                await db.SaveChangesAsync();
                int id = member.Id;

                User user = new User()
                {
                    MemberId = id,
                    Email = $"seedMail{i}@google.com",
                    UserName = $"seedUsername{i}",
                    PasswordHash = passwordHash
                };
                await userRepository.CreateAsync(user);
                await db.SaveChangesAsync();
            }
        }
    }

}
