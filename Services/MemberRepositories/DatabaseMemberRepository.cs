using AuthenticationServer.API.Data;
using AuthenticationServer.API.Entities;

namespace AuthenticationServer.API.Services.MemberRepositories
{
    public class DatabaseMemberRepository : IMemberRepository
    {
        private readonly AuthenticationDbContext db;

        public DatabaseMemberRepository(AuthenticationDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Create(Member member)
        {
            await db.Members.AddAsync(member);
            await db.SaveChangesAsync();
            return member.Id;
        }

        public async Task Delete(int id)
        {
            Member? member = await db.Members.FindAsync(id);
            if (member != null)
            {
                member.DeletedAt = DateTime.Now;
                await db.SaveChangesAsync();
            }
        }

        public async Task<Member> FindById(int id)
        {
            Member? member = await db.Members.FindAsync(id);
            if (member != null)
            {
                return member;
            }
            return null;
        }
    }
}
