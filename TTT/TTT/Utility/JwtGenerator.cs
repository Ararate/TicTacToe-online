using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Text;

namespace TTT.Utility
{
    public class JwtGenerator
    {
        public string GetToken(string username)
        {
            var claim = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var jwt = new JwtSecurityToken(
                issuer: LC.JwtIssuer,
                audience: LC.JwtAudience,
                claims: claim,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials
                    (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LC.JwtSecretKey)), 
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
