using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServer.API.Entities
{
    public class User :IdentityUser<int>
    {
        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Member Member { get; set; }
        public int IsAgreeToTermsOfServiceVersion { get; set; }
    }
}
