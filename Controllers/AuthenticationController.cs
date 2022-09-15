using AuthenticationServer.API.Data;
using AuthenticationServer.API.Dtos.Requests;
using AuthenticationServer.API.Dtos.Responses;
using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.MemberRepositories;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.RefreshValidators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;
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

        public AuthenticationController(UserManager<User> userRepository, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator, IPasswordHasher passwordHasher, IMemberRepository memberRepository, KakaoBackchannelAccessTokenAuthenticator kakaoBackchannelAccessTokenAuthenticator, GoogleBackchannelAccessTokenAuthenticator googleBackchannelAccessTokenAuthenticator, AuthenticationDbContext authenticationDbContext, AppleBackchannelAccessTokenAuthenticator appleBackchannelAccessTokenAuthenticator)
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
                UserName = registerRequest.UserName,
                Password = passwordHash,
                Nickname = registerRequest.Nickname,
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
            return Ok(new { status = "success" });
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

            if (signinOAuthRequest.Provider == "kakao")
            {
                jsonDocument = await kakaoBackchannelAccessTokenAuthenticator.GetUserProfileAsync(signinOAuthRequest.AccessToken);
                var kakaoAccount = jsonDocument.RootElement.GetProperty("kakao_account");
                if (kakaoAccount.GetProperty("has_email").ToString() == "true" && kakaoAccount.GetProperty("is_email_verified").ToString() == "true" && kakaoAccount.GetProperty("is_email_valid").ToString() == "true")
                {
                    var email = kakaoAccount.GetProperty("email").GetString();
                    User existingUserByEmail = await userRepository.FindByEmailAsync(email);

                    if (existingUserByEmail != null)
                    {
                        var userLogin = authenticationDbContext.UserLogins.SingleOrDefault(e => e.UserId == existingUserByEmail.Id && e.LoginProvider == "kakao");
                        if (userLogin == null)//이미 가입된 이메일이 있고, 연결되어 있지 않다면 로그인해서 링크해라
                        {
                            return Conflict(new ErrorResponse("Email already exists. If you already sign up with email and password, use link external provider function"));

                        }
                        else//이미 가입된 이메일이 있고, 연결되어 있다면 로그인 가능하다
                        {
                            AuthenticatedUserResponse response = await authenticator.Authenticate(existingUserByEmail);
                            return Ok(response);
                        }
                    }
                    else //가입한 적 없는 이메일이므로 지금 가입한다.
                    {
                        Member registrationMember = new Member()
                        {
                            CreatedAt = DateTime.UtcNow,
                            Email = email,
                            UserName = email,
                            Nickname = kakaoAccount.GetProperty("profile").GetProperty("nickname").ToString(),
                            Role = "ROLE_USER"
                        };
                        int id = await memberRepository.Create(registrationMember);


                        User registrationUser = new User()
                        {
                            MemberId = id,
                            Email = email,
                            UserName = email,
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
                        var user = await userRepository.FindByEmailAsync(email);
                        await authenticationDbContext.UserLogins.AddAsync(new IdentityUserLogin<int>() { LoginProvider = "kakao", ProviderDisplayName = "kakao", ProviderKey = jsonDocument.RootElement.GetProperty("id").ToString(), UserId = user.Id });
                        await authenticationDbContext.SaveChangesAsync();

                        AuthenticatedUserResponse response = await authenticator.Authenticate(user);
                        return Ok(response);
                    }
                }
                return BadRequest(new ErrorResponse("Kakao didn't provide enough informations"));
                //               {
                //  "id": 2403366545,
                //  "connected_at": "2022-08-25T14:59:12Z",
                //  "for_partner": {
                //    "uuid": "CDsCNwM3BDcGKhgtGiMTJBUkEj4LOgw5CkA"
                //  },
                //  "properties": {
                //    "nickname": "배성재",
                //    "profile_image": "http://k.kakaocdn.net/dn/hGHeu/btrFT3o8ThQ/0zClP7QkQo9LalIzVHTmx1/img_640x640.jpg",
                //    "thumbnail_image": "http://k.kakaocdn.net/dn/hGHeu/btrFT3o8ThQ/0zClP7QkQo9LalIzVHTmx1/img_110x110.jpg"
                //  },
                //  "kakao_account": {
                //    "profile_needs_agreement": false,
                //    "profile": {
                //      "nickname": "배성재",
                //      "thumbnail_image_url": "http://k.kakaocdn.net/dn/hGHeu/btrFT3o8ThQ/0zClP7QkQo9LalIzVHTmx1/img_110x110.jpg",
                //      "profile_image_url": "http://k.kakaocdn.net/dn/hGHeu/btrFT3o8ThQ/0zClP7QkQo9LalIzVHTmx1/img_640x640.jpg",
                //      "is_default_image": false
                //    },
                //    "has_email": true,
                //    "email_needs_agreement": false,
                //    "is_email_valid": true,
                //    "is_email_verified": true,
                //    "email": "hsm0156@kaist.ac.kr",
                //    "has_phone_number": true,
                //    "phone_number_needs_agreement": false,
                //    "phone_number": "+82 10-2379-0156",
                //    "has_birthday": true,
                //    "birthday_needs_agreement": true,
                //    "has_gender": true,
                //    "gender_needs_agreement": false,
                //    "gender": "male",
                //    "is_kakaotalk_user": true
                //  }
                //}
            }

            if (signinOAuthRequest.Provider == "google")
            {
                try { 
                jsonDocument = await googleBackchannelAccessTokenAuthenticator.GetUserProfileAsync(signinOAuthRequest.AccessToken);
                }
                catch(Exception e)
                {
                    return BadRequest(new ErrorResponse("invalid token"));
                }
                    var googleAccount = jsonDocument.RootElement;
                if (googleAccount.GetProperty("email_verified").ToString() == "true")
                {
                    var email = googleAccount.GetProperty("email").GetString();
                    User findByEmail = await userRepository.FindByEmailAsync(email);

                    if (findByEmail != null)
                    {
                        var userLogin = authenticationDbContext.UserLogins.SingleOrDefault(e => e.UserId == findByEmail.Id && e.LoginProvider == "google");
                        if (userLogin == null)//이미 가입된 이메일이 있고, 연결되어 있지 않다면 로그인해서 링크해라
                        {
                            return Conflict(new ErrorResponse("Email already exists. If you already sign up with email and password, use link external provider function"));
                        }
                        else//이미 가입된 이메일이 있고, 연결되어 있다면 로그인 가능하다
                        {
                            AuthenticatedUserResponse response = await authenticator.Authenticate(findByEmail);
                            return Ok(response);
                        }

                    }
                    else //가입한 적 없는 이메일이므로 지금 가입한다.
                    {
                        Member registrationMember = new Member()
                        {
                            CreatedAt = DateTime.UtcNow,
                            Email = email,
                            UserName = email,
                            Nickname = googleAccount.GetProperty("name").ToString(),
                            Role = "ROLE_USER"
                        };
                        int id = await memberRepository.Create(registrationMember);


                        User registrationUser = new User()
                        {
                            MemberId = id,
                            Email = email,
                            UserName = email,
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
                        var user = await userRepository.FindByEmailAsync(email);
                        await authenticationDbContext.UserLogins.AddAsync(new IdentityUserLogin<int>() { LoginProvider = "google", ProviderDisplayName = "google", ProviderKey = jsonDocument.RootElement.GetProperty("sub").ToString(), UserId = user.Id });
                        await authenticationDbContext.SaveChangesAsync();

                        AuthenticatedUserResponse response = await authenticator.Authenticate(user);
                        return Ok(response);
                    }


                    //{
                    //  "iss": "https://accounts.google.com",
                    //  "nbf": "1661439552",
                    //  "aud": "624353687612-fjt6b8kmdfrsmhc9efmps7b1m9ek11l1.apps.googleusercontent.com",
                    //  "sub": "103877988597939757746",
                    //  "email": "hsm0156@gmail.com",
                    //  "email_verified": "true",
                    //  "azp": "624353687612-fjt6b8kmdfrsmhc9efmps7b1m9ek11l1.apps.googleusercontent.com",
                    //  "name": "배성재",
                    //  "picture": "https://lh3.googleusercontent.com/a/AItbvml5lfkoobduPvtvYUwITSfsGwAROc6WnyYCwejf=s96-c",
                    //  "given_name": "성재",
                    //  "family_name": "배",
                    //  "iat": "1661439852",
                    //  "exp": "1661443452",
                    //  "jti": "0a1206626dacb0397120fb7f058653eef84408c1",
                    //  "alg": "RS256",
                    //  "kid": "402f305b70581329ff289b5b3a67283806eca893",
                    //  "typ": "JWT"
                    //}
                }
                return BadRequest(new ErrorResponse("Google didn't provide enough informations"));
            }

            if (signinOAuthRequest.Provider == "apple")
            {
                var token = "a758aa04c21334130a33b363df4cd1eff.0.ssq.1zAHspzyKdEs6MCDHbjeeA";

                jsonDocument = await appleBackchannelAccessTokenAuthenticator.GetUserProfileAsync(signinOAuthRequest.AccessToken);
            }

            return NotFound(new ErrorResponse("no oauth provider"));
        }

        //[Authorize]
        //[HttpPost("linkOAuth")]
        //public async Task<IActionResult> LinkOAuth([FromBody] SigninOAuthRequest signinOAuthRequest)
        //{

        //}

        [Authorize]
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

        [Authorize]
        [HttpDelete("withdrawal")]
        public async Task<ActionResult> Withdrawal()
        {
            string rawEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var user = await userRepository.FindByEmailAsync(rawEmail);
            await memberRepository.Delete(user.MemberId);
            await userRepository.SetLockoutEnabledAsync(user, true);
            await userRepository.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(10));
            return Ok(new { status = "success" });
        }
    }
}