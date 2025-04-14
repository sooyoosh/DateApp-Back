using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DatingApp.Controllers
{

    [ApiController]
    [Route("api/users")]
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UsersController:ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository,IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;

        }

        [HttpGet]
        public async  Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            userParams.CurrentUserName = User.GetUsername();
            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }

        
        [HttpGet("{username}")]

        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("the user not found");
            }
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("Could not found User");
            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("fail to update");
        }

        //cloudinary
        [HttpPost("photo/upload")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto([FromForm] IFormFile file)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("user not found");
            var result = await _photoService.UploadPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUser), new {username=user.UserName},_mapper.Map<PhotoDto>(photo));
            } //return _mapper.Map<PhotoDto>(photo);

            return BadRequest("Problem To Uploading Image");
        }

        [HttpDelete("photo/delete/{publicId}")]
        public async Task<IActionResult> DeletePhoto(string publicId)
        {
            var result = await _photoService.DeletePhotoAsync(publicId);
            if (result.Result == "ok") return Ok(new { message = "Photo deleted successfully" });

            return BadRequest("Failed to delete photo");
        }


        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("Could not find user");
            var photo=user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null||photo.IsMain) return BadRequest("Cannot set main");
            var currentMain=user.Photos.FirstOrDefault(p=>p.IsMain);
            if (currentMain != null) currentMain.IsMain=false;
            photo.IsMain=true;
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("there is problem to save it");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("Could not find user");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("photo cannot be delete");
            if (photo.PublicId != null)
            {
                var result=await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) {return BadRequest(result.Error); } 
            }
            user.Photos.Remove(photo);  
            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("there is problem");

        }
        



    }
}
