using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServer.API.Models
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
