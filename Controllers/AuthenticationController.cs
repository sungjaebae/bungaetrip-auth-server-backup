using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Requests;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.TokenGenerators;
using AuthenticationServer.API.Services.UserRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly AccessTokenGenerator accessTokenGenerator;

        public AuthenticationController(IUserRepository userRepository, IPasswordHasher passwordHasher, AccessTokenGenerator accessTokenGenerator)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.accessTokenGenerator = accessTokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest();
            }

            //겹치는 사용자가 있는가
            User existingUserByEmail = await userRepository.GetByEmail(registerRequest.Email);
            if(existingUserByEmail != null)
            {
                return Conflict();
            }
            User existingUserByUsername = await userRepository.GetByUserName(registerRequest.UserName);
            if(existingUserByUsername != null)
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
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = await userRepository.GetByUserName(loginRequest.UserName);
            if(user == null)
            {
                return Unauthorized();
            }
            bool isCorrectPassword = passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash);
            if(!isCorrectPassword)
            {
                return Unauthorized();
            }
            string accessToken = accessTokenGenerator.GenerateToken(user);
            return Ok(new AuthenticatedUserResponse() { AccessToken = accessToken });
        }
    }
}