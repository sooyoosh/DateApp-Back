using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DatingApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public string CreateToken(AppUser user)
        {
            var TokenKey = _configuration["TokenKey"] ?? throw new Exception("Your not have acces");
            if(TokenKey.Length<64) { throw new Exception("your token key needs to be longer"); }

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var claims =new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),       
            Expires = DateTime.Now.AddDays(7),         
            SigningCredentials = creds                 
        };
            var tokenhandler=new JwtSecurityTokenHandler();
            var token=tokenhandler.CreateToken(tokenDescriptor);

            return tokenhandler.WriteToken(token);
        }
    }
}
