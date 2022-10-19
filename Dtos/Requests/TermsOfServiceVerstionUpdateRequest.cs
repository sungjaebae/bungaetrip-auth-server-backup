using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Dtos.Requests
{
    public class TermsOfServiceVerstionUpdateRequest
    {
        [Required]
        public int UpdatedVersion { get; set; }
    }
}
