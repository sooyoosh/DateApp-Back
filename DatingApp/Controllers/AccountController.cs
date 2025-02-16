using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(DataContext context,ITokenService tokenService):ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            return Ok();

            //if(await UserExist(registerDto.Username)) { return BadRequest("username already taken"); };

            //using var hmac=new HMACSHA512();
            //var user = new AppUser()
            //{
            //    UserName = registerDto.Username.ToLower(),
            //    PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //    PasswordSalt=hmac.Key

            //};
            //context.Users.Add(user);
            //await context.SaveChangesAsync();

            //return Ok(new UserDto {
            //Username=user.UserName,
            //Token=tokenService.CreateToken(user)
            //});

        }

        private async Task<bool> UserExist(string userName)
        {
            return await context.Users.AnyAsync(x=>x.UserName.ToLower() == userName.ToLower());
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.UserName==loginDto.Username.ToLower());

            if(user==null) { return Unauthorized("Invalid username"); }

            using var hmac=new HMACSHA512(user.PasswordSalt);
            var copmuteHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < copmuteHash.Length; i++)
            {
                if (copmuteHash[i] != user.PasswordHash[i]) { return Unauthorized("Invalid Password"); }
            }

            return Ok(new UserDto
            {
                Username= user.UserName,
                Token=tokenService.CreateToken(user)
            }); 

        }





    }
}
