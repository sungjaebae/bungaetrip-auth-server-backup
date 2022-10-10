using AuthenticationServer.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthenticationServer.API.Controllers
{
    [ApiController]
    [Route("auth/[controller]")]
    public class AppSettingAndSecretController : ControllerBase
    {

        private readonly ILogger<AppSettingAndSecretController> _logger;
        private readonly AppSettings appSettings;
        private readonly AppSecrets appSecrets;

        public AppSettingAndSecretController(ILogger<AppSettingAndSecretController> logger, IOptions<AppSettings> appSettings, IConfiguration configuration )
        {
            _logger = logger;
            this.appSettings = appSettings.Value;
            this.appSecrets = configuration.GetSection(nameof(AppSecrets)).Get<AppSecrets>();
        }

        [HttpGet(Name = "AppSettingAndSecret")]
        public IActionResult Get()
        {
            return Ok(new { appSettings, appSecrets });
        }
    }
}