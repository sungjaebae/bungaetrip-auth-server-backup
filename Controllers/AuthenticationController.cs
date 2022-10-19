using AuthenticationServer.API.Data;
using AuthenticationServer.API.Dtos;
using AuthenticationServer.API.Dtos.Requests;
using AuthenticationServer.API.Dtos.Responses;
using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Migrations;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.MemberRepositories;
using AuthenticationServer.API.Services.NicknameGenerators;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.RefreshValidators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace AuthenticationServer.API.Controllers
{
    [Route("auth/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> userRepository;
        private readonly RefreshTokenValidator refreshTokenValidator;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly Authenticator authenticator;
        private readonly IPasswordHasher passwordHasher;
        private readonly IMemberRepository memberRepository;
        private readonly KakaoBackchannelAccessTokenAuthenticator kakaoBackchannelAccessTokenAuthenticator;
        private readonly GoogleBackchannelAccessTokenAuthenticator googleBackchannelAccessTokenAuthenticator;
        private readonly AppleBackchannelAccessTokenAuthenticator appleBackchannelAccessTokenAuthenticator;
        private readonly AuthenticationDbContext authenticationDbContext;
        private readonly NicknameGenerator nicknameGenerator;

        public AuthenticationController(UserManager<User> userRepository, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator, IPasswordHasher passwordHasher, IMemberRepository memberRepository, KakaoBackchannelAccessTokenAuthenticator kakaoBackchannelAccessTokenAuthenticator, GoogleBackchannelAccessTokenAuthenticator googleBackchannelAccessTokenAuthenticator, AuthenticationDbContext authenticationDbContext, AppleBackchannelAccessTokenAuthenticator appleBackchannelAccessTokenAuthenticator, NicknameGenerator nicknameGenerator)
        {
            this.userRepository = userRepository;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenRepository = refreshTokenRepository;
            this.authenticator = authenticator;
            this.passwordHasher = passwordHasher;
            this.memberRepository = memberRepository;
            this.kakaoBackchannelAccessTokenAuthenticator = kakaoBackchannelAccessTokenAuthenticator;
            this.googleBackchannelAccessTokenAuthenticator = googleBackchannelAccessTokenAuthenticator;
            this.authenticationDbContext = authenticationDbContext;
            this.appleBackchannelAccessTokenAuthenticator = appleBackchannelAccessTokenAuthenticator;
            this.nicknameGenerator = nicknameGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //겹치는 사용자가 있는가
            User existingUserByEmail = await userRepository.FindByEmailAsync(registerRequest.Email);
            if (existingUserByEmail != null)
            {
                return Conflict(new ErrorResponse("Email already exists."));
            }
            User existingUserByUsername = await userRepository.FindByNameAsync(registerRequest.UserName);
            if (existingUserByUsername != null)
            {
                return Conflict(new ErrorResponse("Username already exists."));
            }

            var passwordHash = passwordHasher.HashPassword(registerRequest.Password);
            Member registrationMember = new Member()
            {
                CreatedAt = DateTime.UtcNow,
                Email = registerRequest.Email,
                UserName = registerRequest.UserName ?? registerRequest.Email,
                Password = passwordHash,
                Nickname = registerRequest.Nickname ?? nicknameGenerator.generateNickname(),
                Role = "ROLE_USER"
            };
            int id = await memberRepository.Create(registrationMember);


            User registrationUser = new User()
            {
                MemberId = id,
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                PasswordHash = passwordHash,
            };
            IdentityResult result = await userRepository.CreateAsync(registrationUser);
            if (!result.Succeeded)
            {
                IdentityErrorDescriber errorDescriber = new IdentityErrorDescriber();
                IdentityError primaryError = result.Errors.FirstOrDefault();
                if (primaryError.Code == nameof(errorDescriber.DuplicateEmail))
                {
                    return Conflict(new ErrorResponse("Email already exists."));
                }
                if (primaryError.Code == nameof(errorDescriber.DuplicateUserName))
                {
                    return Conflict(new ErrorResponse("Username already exists."));
                }

            }
            return Ok(new { status = "success", IsAgreeToTermsOfService = false });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedUserResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = await userRepository.FindByNameAsync(loginRequest.UserName);
            if (user == null)
            {
                return Unauthorized();
            }
            if (DateTime.Today < user.LockoutEnd)
            {
                return Unauthorized(new { status = "fail", description = "widthdrawal user" });
            }

            bool isCorrectPassword = passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return Unauthorized();
            }
            AuthenticatedUserResponse response = await authenticator.Authenticate(user);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticatedUserResponse>> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isValidRefreshToken = refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token."));
            }
            RefreshToken refreshTokenDto = await refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshTokenDto == null)
            {
                return NotFound(new ErrorResponse("Invalid refresh token"));
            }

            await refreshTokenRepository.Delete(refreshTokenDto.Id);

            User user = await userRepository.FindByIdAsync(refreshTokenDto.UserId.ToString());
            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            AuthenticatedUserResponse response = await authenticator.Authenticate(user);
            return Ok(response);
        }

        [HttpPost("signinOAuth")]
        public async Task<ActionResult<AuthenticatedUserResponse>> SigninOAuth([FromBody] SigninOAuthRequest signinOAuthRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            JsonDocument? jsonDocument = null;
            string username = null;
            string email = null;

            if (signinOAuthRequest.Provider == "Kakao")
            {
                signinOAuthRequest.Provider = "KakaoTalk";

                try
                {
                    jsonDocument = await kakaoBackchannelAccessTokenAuthenticator.GetUserProfileAsync(signinOAuthRequest.AccessToken);
                }
                catch (Exception e)
                {
                    return BadRequest(new ErrorResponse("invalid token"));
                }
                var kakaoAccount = jsonDocument.RootElement.GetProperty("kakao_account");
                var kakaoId = jsonDocument.RootElement.GetProperty("id").GetInt64();

                if (kakaoAccount.GetProperty("has_email").ToString() == "true" && kakaoAccount.GetProperty("is_email_verified").ToString() == "true" && kakaoAccount.GetProperty("is_email_valid").ToString() == "true")
                {
                    email = kakaoAccount.GetProperty("email").GetString();
                }
                username = signinOAuthRequest.Provider + kakaoId;
            }

            if (signinOAuthRequest.Provider == "Google")
            {
                try
                {
                    jsonDocument = await googleBackchannelAccessTokenAuthenticator.GetUserProfileAsync(signinOAuthRequest.AccessToken);
                }
                catch (Exception e)
                {
                    return BadRequest(new ErrorResponse("invalid token"));
                }
                var googleAccount = jsonDocument.RootElement;
                if (googleAccount.GetProperty("email_verified").ToString() == "true")
                {
                    email = googleAccount.GetProperty("email").GetString();
                }
                username = signinOAuthRequest.Provider + googleAccount.GetProperty("sub").GetString();
            }

            if (signinOAuthRequest.Provider == "Apple")
            {

                var publicKey = await appleBackchannelAccessTokenAuthenticator.GetPublicKeyAsync();
                //var jwks = await new HttpClient().GetStringAsync("https://appleid.apple.com/auth/keys");
                var validationParameters = new TokenValidationParameters
                {
                    IssuerSigningKeys = new JsonWebKeySet(publicKey).Keys,
                    ValidAudience = "aud", 
                    ValidIssuer = "iss",
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var claimsPrincipal = tokenHandler.ValidateToken(signinOAuthRequest.AccessToken, validationParameters, out SecurityToken securityToken);
                    if (securityToken == null)
                    {
                        return BadRequest(new ErrorResponse("Apple Login Failed"));
                    }
                    username = signinOAuthRequest.Provider+ claimsPrincipal.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
                    email = claimsPrincipal.Claims.First(u => u.Type == ClaimTypes.Email).Value;
                }
                catch (Exception ex)
                {
                    // 인증 실패
                    return BadRequest(new ErrorResponse("Apple Login Failed")); ;
                }
            }

            if (username == null)
            { 
                return NotFound(new ErrorResponse("no oauth provider"));
            }
            //기존에 외부 로그인한 사용자인지 확인한다
            User user = await userRepository.FindByNameAsync(username);
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
                IdentityResult result = await userRepository.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            Member member= await memberRepository.FindById(user.MemberId);
            if (member.DeletedAt != null)
            {
                return Unauthorized(new { status = "fail", description = "widthdrawal user" });
            }
            AuthenticatedUserResponse response = await authenticator.Authenticate(user);

            return Ok(response);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");
            if (!int.TryParse(rawUserId, out int userId))
            {
                return Unauthorized();
            }
            await refreshTokenRepository.DeleteAll(userId);
            return Ok(new { status = "success" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("withdrawal")]
        public async Task<ActionResult> Withdrawal()
        {
            string rawUsername = HttpContext.User.FindFirstValue("username");

            var user = await userRepository.FindByNameAsync(rawUsername);
            var member = await memberRepository.FindById(user.MemberId);
            if(member.DeletedAt!=null)
            {
                return Conflict(new { status = "fail", description = "already widthdrawal user" });
            }
            await memberRepository.Delete(user.MemberId);
            return Ok(new { status = "success" });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("TermsOfServiceVersion")]
        public async Task<ActionResult> TermsOfServiceVersion([FromBody]TermsOfServiceVerstionUpdateRequest termsOfServiceVerstionUpdateRequest)
        {
            try
            {
                string rawUsername = HttpContext.User.FindFirstValue("username");

                var user = await userRepository.FindByNameAsync(rawUsername);
                user.IsAgreeToTermsOfServiceVersion = termsOfServiceVerstionUpdateRequest.UpdatedVersion;
                await userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return Conflict(new { status = "fail", description = "version may invalid" });
            }
            return Ok(new { status = "success" });
        }
    }
}