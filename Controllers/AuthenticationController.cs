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
using System.Security.Claims;

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

        public AuthenticationController(UserManager<User> userRepository, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator, IPasswordHasher passwordHasher, IMemberRepository memberRepository)
        {
            this.userRepository = userRepository;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenRepository = refreshTokenRepository;
            this.authenticator = authenticator;
            this.passwordHasher = passwordHasher;
            this.memberRepository = memberRepository;
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
            if(existingUserByEmail != null)
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
            if(!result.Succeeded)
            {
                IdentityErrorDescriber errorDescriber = new IdentityErrorDescriber();
                IdentityError primaryError = result.Errors.FirstOrDefault();
                if(primaryError.Code == nameof(errorDescriber.DuplicateEmail))
                {
                    return Conflict(new ErrorResponse("Email already exists."));
                }
                if(primaryError.Code == nameof(errorDescriber.DuplicateUserName))
                {
                    return Conflict(new ErrorResponse("Username already exists."));
                }

            }
            return Ok(new { status = "success"});
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

        [Authorize]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");
            if(!int.TryParse(rawUserId, out int userId))
            {
                return Unauthorized();
            }
            await refreshTokenRepository.DeleteAll(userId);
            return NoContent();
        }
    }
}