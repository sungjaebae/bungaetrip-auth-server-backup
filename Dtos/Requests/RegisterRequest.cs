using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Dtos.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Nickname { get; set; }


    }
}
