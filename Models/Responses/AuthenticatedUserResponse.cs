using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Models.Responses
{
    public class AuthenticatedUserResponse
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
