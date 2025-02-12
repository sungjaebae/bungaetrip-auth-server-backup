﻿using AuthenticationServer.API.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AuthenticationServer.API.Models.AppSecrets;

namespace AuthenticationServer.API.Services.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly JwtConfigurations _configuration;

        public RefreshTokenGenerator(JwtConfigurations configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken()
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(_configuration.RefreshTokenExpirationMinutes);

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.RefreshTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(_configuration.Issuer, _configuration.Audience, null, DateTime.UtcNow, expirationTime, credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
