using System.Text.Json;

namespace AuthenticationServer.API.Models
{
    public class AppSettings
    {
        public string[] AllowedOrigins { get; set; }
        public string BaseUrl { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
