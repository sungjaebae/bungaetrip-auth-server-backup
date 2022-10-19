using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Services.PasswordHashers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AuthenticationServer.API.Data
{
    public class AuthenticationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            BcryptPasswordHasher passwordHasher = new BcryptPasswordHasher();
            var passwordHash = passwordHasher.HashPassword("1234");
            string[] lastName = { "김", "이", "박", "최", "한", "오", "배" };
            string[] firstName = { "성재", "형수", "진석", "두훈", "저스틴", "한별" };
            Random rand = new Random();
            string[] gender = { "MALE", "FEMALE" };
            List<User> users = new List<User> { };
            List<Member> members = new List<Member> { };
            for (int i = 1; i <= 10; i++)
            {
                Member member = new Member()
                {
                    CreatedAt = DateTime.UtcNow,
                    Email = $"seedMail{i}@google.com",
                    UserName = $"seedUsername{i}",
                    Nickname = lastName[rand.Next(0, 5)] + firstName[rand.Next(0, 6)],
                    Password = passwordHash,
                    Gender = gender[rand.Next(0, 2)],
                    Role = "ROLE_USER",
                    Id = i
                };
                members.Add(member);
            }
            builder.Entity<Member>().HasData(members);

            for (int i = 1; i <= 10; i++)
            { 
                User user = new User()
                {
                    MemberId = i,
                    Email = $"seedMail{i}@google.com",
                    UserName = $"seedUsername{i}",
                    PasswordHash = passwordHash,
                    Id = i
                };
                users.Add(user);
            }
            builder.Entity<User>().HasData(users);
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Member> Members { get; set; }
    }
}
