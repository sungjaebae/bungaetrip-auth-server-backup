using AuthenticationServer.API.Entities;

namespace AuthenticationServer.API.Services.MemberRepositories
{
    public interface IMemberRepository
    {
        Task<int> Create(Member member);
        Task Delete(int id);
        Task<Member> FindById(int id);
    }
}
