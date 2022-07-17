using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Services.RefreshTokenRepositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByToken(string token);
        Task Create(RefreshToken refreshToken);
        Task Delete(int id);
        Task DeleteAll(int userId);
    }
}
