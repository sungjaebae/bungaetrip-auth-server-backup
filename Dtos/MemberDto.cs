using System.Text.Json.Serialization;

namespace AuthenticationServer.API.Dtos
{
    public class MemberDto
    {
        public int Id { get; set; }
        public int? Age { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Nickname { get; set; }
        public string? Username { get; set; }
    }
}
