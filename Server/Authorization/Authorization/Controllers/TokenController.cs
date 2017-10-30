using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Linq;

namespace Authorization.Controllers
{
    [Route("api/token")]
    public class TokenController : Controller
    {
        private readonly UserManager<User> _userManager;

        public TokenController(UserManager<User> userManager) => _userManager = userManager;
        
        [HttpPost, Route("create")]
        public async Task Create(string username, string password)
        {
            var identity = GetIdentity(username, password).Result;
            
            if (identity == null)
            {
                //to do error
            }
            
            var nowTime = DateTime.UtcNow;
            
            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: nowTime,
                claims: identity.Claims,
                expires: nowTime.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            
            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(response,
                new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            var users = _userManager.Users.ToList();

            if (user == null || !await _userManager.CheckPasswordAsync(user, password)) return null;
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Nbf, 
                    new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, 
                    new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
            };
                
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}