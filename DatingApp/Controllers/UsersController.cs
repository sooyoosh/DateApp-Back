using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UsersController(DataContext context):ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async  Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await context.Users.ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]

        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("the user not found");
            }
            return Ok(user);
        }
    }
}
