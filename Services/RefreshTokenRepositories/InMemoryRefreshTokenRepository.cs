using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.RefreshTokenRepositories
{
    public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> refreshTokens= new List<RefreshToken>();
        public Task Create(RefreshToken refreshToken)
        {
            refreshToken.Id = refreshTokens.Count + 1;
            refreshTokens.Add(refreshToken);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            refreshTokens.RemoveAll(r => r.Id == id);
            return Task.CompletedTask;
        }

        public Task DeleteAll(int userId)
        {
            refreshTokens.RemoveAll(r => r.UserId == userId);
            return Task.CompletedTask;
        }

        public Task<RefreshToken> GetByToken(string token)
        {
            RefreshToken refreshToken = refreshTokens.FirstOrDefault(r => r.Token == token);
            return Task.FromResult(refreshToken);
        }
    }
}
