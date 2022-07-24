using AuthenticationServer.API.Entities;
using AuthenticationServer.API.Models.Requests;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.RefreshValidators;
using AuthenticationServer.API.Services.TokenGenerators;
using AuthenticationServer.API.Services.UserRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly RefreshTokenValidator refreshTokenValidator;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly Authenticator authenticator;

        public AuthenticationController(IUserRepository userRepository, IPasswordHasher passwordHasher, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenRepository = refreshTokenRepository;
            this.authenticator = authenticator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //겹치는 사용자가 있는가
            User existingUserByEmail = await userRepository.GetByEmail(registerRequest.Email);
            if (existingUserByEmail != null)
            {
                return Conflict();
            }
            User existingUserByUsername = await userRepository.GetByUserName(registerRequest.UserName);
            if (existingUserByUsername != null)
            {
                return Conflict();
            }
            string passwordHash = passwordHasher.HashPassword(registerRequest.Password);
            User registrationUser = new User()
            {
                Email = registerRequest.Email,
                UserName = registerRequest.UserName,
                PasswordHash = passwordHash
            };
            var user = await userRepository.Create(registrationUser);
            return Ok(user); //passwordHash가 공개되므로 user를 내려주지 않아야 하나
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedUserResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = await userRepository.GetByUserName(loginRequest.UserName);
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

            User user = await userRepository.GetById(refreshTokenDto.UserId);
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