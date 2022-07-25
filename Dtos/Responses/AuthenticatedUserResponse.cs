using AuthenticationServer.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Dtos.Responses
{
    public class AuthenticatedUserResponse
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public DateTime AccessTokenExpirationTime { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public MemberDto Member { get; set; }
    }
}
