using AuthenticationServer.API.Dtos;
using AuthenticationServer.API.Dtos.Responses;
using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.MemberRepositories;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.TokenGenerators;

namespace AuthenticationServer.API.Services.Authenticators
{
    public class Authenticator
    {
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IMemberRepository memberRepository;

        public Authenticator(AccessTokenGenerator accessTokenGenerator,
            RefreshTokenGenerator refreshTokenGenerator,
            IRefreshTokenRepository refreshTokenRepository,
            IMemberRepository memberRepository)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
            this.memberRepository = memberRepository;
        }

        public async Task<AuthenticatedUserResponse> Authenticate(User user)
        {
            AccessToken accessToken = _accessTokenGenerator.GenerateToken(user);
            string refreshToken = _refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };
            await _refreshTokenRepository.Create(refreshTokenDTO);

            Member member = await memberRepository.FindById(user.MemberId);
            MemberDto memberDto = new MemberDto()
            {
                Id = member.Id,
                Age = member.Age,
                Description = member.Description,
                Email = member.Email,
                Gender = member.Gender,
                Nickname = member.Nickname,
                Username = member.UserName
            };

            return new AuthenticatedUserResponse()
            {
                AccessToken = accessToken.Value,
                AccessTokenExpirationTime = accessToken.ExpirationTime,
                RefreshToken = refreshToken,
                Member = memberDto
            };
        }
    }
}
