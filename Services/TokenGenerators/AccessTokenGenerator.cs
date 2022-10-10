using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AuthenticationServer.API.Models.AppSecrets;

namespace AuthenticationServer.API.Services.TokenGenerators
{
    public class AccessTokenGenerator
    {
        private readonly JwtConfigurations _configuration;

        public AccessTokenGenerator(JwtConfigurations configuration)
        {
            _configuration = configuration;
        }

        public AccessToken GenerateToken(User user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.AccessTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.MemberId.ToString()),
                new Claim("email", user.Email??"OAuth"),
                new Claim("username", user.UserName)
            };
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(_configuration.AccessTokenExpirationMinutes);
            JwtSecurityToken token = new JwtSecurityToken(_configuration.Issuer, _configuration.Audience, claims, DateTime.UtcNow, expirationTime, credentials);
            string value= new JwtSecurityTokenHandler().WriteToken(token);

            return new AccessToken()
            {
                Value = value,
                ExpirationTime = expirationTime
            };
        }
    }
}
