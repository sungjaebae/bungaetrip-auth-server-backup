using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Dtos.Requests
{
    public class SigninOAuthRequest
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string Provider { get; set; }
    }
}