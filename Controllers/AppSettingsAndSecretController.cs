using AuthenticationServer.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IHostEnvironment hostEnvironment;

        public AppSettingAndSecretController(ILogger<AppSettingAndSecretController> logger, IOptions<AppSettings> appSettings, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            this.appSettings = appSettings.Value;
            this.appSecrets = configuration.GetSection(nameof(AppSecrets)).Get<AppSecrets>();
            this.hostEnvironment = hostEnvironment;
        }

        [HttpGet(Name = "AppSettingAndSecret")]
        public IActionResult Get()
        {
            if (hostEnvironment.IsProduction() || hostEnvironment.IsStaging() || hostEnvironment.IsDevelopment())
            {
                return Ok(new { status = "no secret output" });
            }
            return Ok(new { appSettings, appSecrets });
        }
    }
}