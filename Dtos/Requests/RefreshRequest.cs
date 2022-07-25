using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Dtos.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
