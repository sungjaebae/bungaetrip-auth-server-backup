using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServer.API.Entities
{
    [Table("member")]
    public class Member
    {
        [Key]
        [Column("member_id", TypeName = "bigint(19)")]
        public int Id { get; set; }
        [Column("age", TypeName = "int(10)")]
        public int? Age { get; set; }
        [Column("created_at", TypeName = "datetime(6)")]
        public DateTimeOffset? CreatedAt { get; set; }
        [Column("deleted_at", TypeName = "datetime(6)")]
        public DateTimeOffset? DeletedAt { get; set; }
        [Column("description", TypeName = "text")]
        public string? Description { get; set; }
        [Column("email", TypeName = "varchar(255)")]
        public string? Email { get; set; }
        [Column("gender", TypeName = "varchar(255)")]
        public string? Gender { get; set; }
        [Column("nickname", TypeName = "varchar(255)")]
        public string? Nickname { get; set; }
        [Column("password", TypeName = "varchar(255)")]
        public string? Password { get; set; }
        [Column("role", TypeName = "varchar(255)")]
        public string? Role { get; set; }
        [Column("username", TypeName = "varchar(255)")]
        public string? UserName { get; set; }
    }
}
