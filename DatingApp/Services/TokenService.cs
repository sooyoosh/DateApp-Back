using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace DatingApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration,UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }



        public async Task<string> CreateToken(AppUser user)
        {
            var TokenKey = _configuration["TokenKey"] ?? throw new Exception("Your not have acces");
            if(TokenKey.Length<64) { throw new Exception("your token key needs to be longer"); }

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            if (user.UserName == null) throw new Exception("no username for user");

            var claims =new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


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
