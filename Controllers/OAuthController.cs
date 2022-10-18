using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Services.NicknameGenerators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using AuthenticationServer.API.Models;
using Microsoft.Extensions.Options;
using AuthenticationServer.API.Dtos.Responses;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Dtos.Requests;

namespace AuthenticationServer.API.Controllers
{
    [Route("auth/Authentication")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly string baseUrl;
        private readonly ILogger<OAuthController> logger;
        private readonly NicknameGenerator nicknameGenerator;
        private readonly Authenticator authenticator;

        public AppSettings settings { get; }
        public SignInManager<User> signInManager { get; }
        public UserManager<User> userManager { get; }

        public OAuthController(IOptions<AppSettings> settings, SignInManager<User> signInManager, ILogger<OAuthController> logger, UserManager<User> userManager, Authenticator authenticator, NicknameGenerator nicknameGenerator)
        {
            this.settings = settings.Value;
            this.baseUrl = this.settings.BaseUrl;
            this.signInManager = signInManager;
            this.logger = logger;
            this.userManager = userManager;
            this.authenticator = authenticator;
            this.nicknameGenerator = nicknameGenerator;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("external-login")]
        public IActionResult ExternalLogin([FromQuery] string provider)
        {
            if (provider == "Kakao" || provider == "kakao")
            {
                provider = "KakaoTalk";
            }
            var redirectUrl = $"{baseUrl}/auth/Authentication/external-auth-callback";
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("external-auth-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string? remoteError = null)
        {
            if (remoteError != null)
            {
                return BadRequest("External login failed.");
            }
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return BadRequest("Can't fetch user data from Google.");
            }
            string username = info.LoginProvider + info.ProviderKey;
            string email = info.Principal.Claims.First(u => u.Type == ClaimTypes.Email).Value;
            //기존에 외부 로그인한 사용자인지 확인한다
            User user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                Member registrationMember = new Member()
                {
                    CreatedAt = DateTime.UtcNow,
                    UserName = username,
                    Email = email,
                    Nickname = nicknameGenerator.generateNickname(),
                    Role = "ROLE_USER"
                };
                user = new User()
                {
                    Member = registrationMember,
                    UserName = username,
                    Email = email
                };
                IdentityResult result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            if (DateTime.Today < user.LockoutEnd)
            {
                return Unauthorized(new { status = "fail", description = "widthdrawal user" });
            }
            AuthenticatedUserResponse response = await authenticator.Authenticate(user,false);

            return Ok(response);
        }
    }

}
