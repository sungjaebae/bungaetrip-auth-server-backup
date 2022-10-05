using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using AuthenticationServer.API.Entities;

namespace AuthenticationServer.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private string URL = "https://slowtest.ml";
        private readonly SignInManager<User> signInManager;

        public OAuthController(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        [HttpGet("google")]

        public async Task<IActionResult> Google()
        {
            var redirectUri = Url.Action(nameof(GoogleResponse), "OAuth", new { }, "https");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUri);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }   
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            return Ok(claims);
        }

        [HttpGet("apple")]
        public async Task<IActionResult> Apple()
        {
            return Ok(new { status = "success", message = "apple" });
        }
        [HttpGet("kakao")]
        public async Task<IActionResult> Kakao()
        {
            return Ok(new { status = "success", message = "kakao" });
        }
    }
}
