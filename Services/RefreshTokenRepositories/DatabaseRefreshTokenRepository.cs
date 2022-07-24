using AuthenticationServer.API.Data;
using AuthenticationServer.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.API.Services.RefreshTokenRepositories
{
    public class DatabaseRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthenticationDbContext db;

        public DatabaseRefreshTokenRepository(AuthenticationDbContext db)
        {
            this.db = db;
        }

        public async Task Create(RefreshToken refreshToken)
        {
            db.RefreshTokens.Add(refreshToken);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            RefreshToken refreshToken = await db.RefreshTokens.FindAsync(id);
            if (refreshToken != null)
            {
                db.RefreshTokens.Remove(refreshToken);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAll(int userId)
        {
            IEnumerable<RefreshToken> refreshTokens = await db.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();

            db.RefreshTokens.RemoveRange(refreshTokens);
            await db.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
