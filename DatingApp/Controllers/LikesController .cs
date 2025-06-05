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
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(ILikesRepository likesRepository, IUnitOfWork unitOfWork)
        {
            _likesRepository = likesRepository;
            _unitOfWork = unitOfWork;

        }


        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId=User.GetUserId();
            if (sourceUserId == targetUserId) return BadRequest("you cannot like yourself");
            var existingLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);
            if (existingLike != null) {
                _unitOfWork.LikesRepository.DeleteLike(existingLike);

                if (await _unitOfWork.Complete()) return Ok();

            }

            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
            };
            _unitOfWork.LikesRepository.AddLike(like);

            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("fail to like");
        }

        [HttpGet("likeList")]
        public async Task<ActionResult<IEnumerable<int>>> GetUserLIkeList()
        {
            return Ok(await _unitOfWork.LikesRepository.GetCurrentUserLikeIda(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery]Likesparams likesparams)
        {
            likesparams.UserId = User.GetUserId();

            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesparams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }
    }
}
