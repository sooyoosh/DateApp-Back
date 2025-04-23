using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.Controllers
{
        [ApiController]
        [Route("api/likes")]
        [Authorize]
    public class LikesController:ControllerBase
    {
        private readonly  ILikesRepository _likesRepository;
        public LikesController(ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
        }


        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId=User.GetUserId();
            if (sourceUserId == targetUserId) return BadRequest("you cannot like yourself");
            var existingLike = await _likesRepository.GetUserLike(sourceUserId, targetUserId);
            if (existingLike != null) {
                _likesRepository.DeleteLike(existingLike);

                if (await _likesRepository.SaveChanges()) return Ok();

            }

            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
            };
            _likesRepository.AddLike(like);

            if (await _likesRepository.SaveChanges()) return Ok();
            return BadRequest("fail to like");
        }

        [HttpGet("likeList")]
        public async Task<ActionResult<IEnumerable<int>>> GetUserLIkeList()
        {
            return Ok(await _likesRepository.GetCurrentUserLikeIda(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery]Likesparams likesparams)
        {
            likesparams.UserId = User.GetUserId();

            var users = await _likesRepository.GetUserLikes(likesparams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }
    }
}
