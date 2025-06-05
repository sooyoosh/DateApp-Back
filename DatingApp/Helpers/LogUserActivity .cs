using DatingApp.Data;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DatingApp.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            // Execute the action first
            var resultContext = await next();

            // If user is not authenticated, return
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            // Get user name from claims
            var userId = resultContext.HttpContext.User.GetUserId();

            // Get the repository
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();

            // Find the user
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);

            if (user == null) return;   


            // Update last active
            user.LastActive = DateTime.UtcNow;

            // Save changes
            await unitOfWork.Complete();




        }
    }
}
