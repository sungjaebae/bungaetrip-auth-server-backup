using System.Text.Json;

namespace AuthenticationServer.API.Models
{
    public class AppSecrets
    {
        public Google GoogleSecrets { get; set; }
        public Kakao KakaoSecrets { get; set; }
        public Apple AppleSecrets { get; set; }
        public Connections ConnectionStrings { get; set; }
        public SmtpSetting SmtpSettings { get; set; }
        public JwtConfigurations JwtConfiguration { get; set; }

        public class SmtpSetting
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
        }

        public class Google
        {
            public string AuthClientId { get; set; }
            public string AuthClientSecret { get; set; }
        }
        public class Kakao
        {
            public string AuthClientId { get; set; }
            public string AuthClientSecret { get; set; }
        }
        public class Apple
        {
            public string AuthClientId { get; set; }
            public string AuthTeamId { get; set; }
            public string keyID { get; set; }
        }

        public class Connections
        {
            public string DbConnectionString { get; set; }
        }

        public class JwtConfigurations
        {
            public string AccessTokenSecret { get; set; }
            public double AccessTokenExpirationMinutes { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public string RefreshTokenSecret { get; set; }
            public double RefreshTokenExpirationMinutes { get; set; }
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
