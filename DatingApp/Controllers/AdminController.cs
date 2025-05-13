using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{


    [ApiController]
    [Route("api/admin")]
    
    public class AdminController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("get-roles")]
        [Authorize(Policy= "RequiredAdminRole")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    UserName = x.UserName,
                    Roles = x.UserRoles.Select(x => x.Role.Name).ToList()
                }).ToListAsync();

            return Ok(users);
        }
        [HttpPost("edit-roles/{username}")]
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<ActionResult> EditRoles(string username,string roles)
        {
            if(string.IsNullOrEmpty(roles)) return BadRequest("you must choose one role atleast");
            var user= await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("user not found");
            var selectedRoles=roles.Split(",").ToArray();
            var userRoles=await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("failed to add roles");
            result=await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("failed to remove roles");
            return Ok(await _userManager.GetRolesAsync(user));

        }

    }
}
