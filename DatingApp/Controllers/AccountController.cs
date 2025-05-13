using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,ITokenService tokenService,IMapper mapper):ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExist(registerDto.Username)) { return BadRequest("username already taken"); };

            //using var hmac = new HMACSHA512();

            var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.PasswordRegister));
            //user.PasswordSalt = hmac.Key;

            //context.Users.Add(user);
            //await context.SaveChangesAsync();
            var result = await userManager.CreateAsync(user, registerDto.PasswordRegister);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender= user.Gender
            });
        }

        private async Task<bool> UserExist(string userName)
        {
            return await userManager.Users.AnyAsync(x => x.NormalizedUserName == userName.ToUpper());
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.Users.Include(x=>x.Photos).FirstOrDefaultAsync(x=>x.NormalizedUserName==loginDto.Username.ToUpper());

            if(user==null) { return Unauthorized("Invalid username"); }
            if (user.UserName == null) throw new Exception("no username for user");
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);


            if (!result.Succeeded) return Unauthorized("Invalid password");
            //using var hmac=new HMACSHA512(user.PasswordSalt);
            //var copmuteHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //for(int i = 0; i < copmuteHash.Length; i++)
            //{
            //    if (copmuteHash[i] != user.PasswordHash[i]) { return Unauthorized("Invalid Password"); }
            //}

            return Ok(new UserDto
            {
                Username= user.UserName,
                Token= await tokenService.CreateToken(user),
                PhotoUrl=user.Photos.FirstOrDefault(p=>p.IsMain)?.Url,
                KnownAs=user.KnownAs,
                Gender = user.Gender
            }); 

        }





    }
}
