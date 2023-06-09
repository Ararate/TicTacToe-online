﻿using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Text;
using TTT.Models;
using System.Security.Cryptography;

namespace TTT.Utility
{
    public class JwtService
    {
        public string GetToken(string username)
        {
            var claim = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var jwt = new JwtSecurityToken(
                issuer: LC.JwtIssuer,
                audience: LC.JwtAudience,
                claims: claim,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials
                    (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LC.JwtSecretKey)), 
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public string? GetName(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = LC.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = LC.JwtAudience,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LC.JwtSecretKey)),
                ValidateIssuerSigningKey = true,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            if (principal == null || securityToken == null)
                return null;
            var jwt = (JwtSecurityToken)securityToken;
            return principal.FindFirstValue(ClaimTypes.Name);
        }
    }
}
