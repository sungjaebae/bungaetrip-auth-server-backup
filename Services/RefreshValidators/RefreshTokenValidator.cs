﻿using AuthenticationServer.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthenticationServer.API.Services.RefreshValidators
{
    public class RefreshTokenValidator
    {
        private readonly AuthenticationConfiguration configuration;

        public RefreshTokenValidator(AuthenticationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Validate(string refreshToken)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.RefreshTokenSecret)),
                ValidIssuer = configuration.Issuer,
                ValidAudience = configuration.Audience,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
