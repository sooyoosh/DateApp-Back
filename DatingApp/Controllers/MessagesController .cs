using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{

    [ApiController]
    [Route("api/messages")]
    [Authorize]

    public class MessagesController: ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public MessagesController(IUserRepository userRepository, IMapper mapper,IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;       
            _messageRepository = messageRepository;
        }



        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessage)
        {
            var username = User.GetUsername(); 

            if (username == createMessage.RecipientUserName.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender=await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessage.RecipientUserName);




            var message = new Message
            {
                Sender=sender,
                SenderUsername = username,
                Recipient=recipient,
                RecipientUsername = createMessage.RecipientUserName,
                Content = createMessage.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
                return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            
            messageParams.Username = User.GetUsername(); 

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages);

            return Ok(messages);
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername=User.GetUsername();
            var result = _messageRepository.GetMessageThread(currentUsername, username);
            return Ok(result.Result);
        }





        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userName=User.GetUsername();
            var message = await _messageRepository.GetMessage(id);
            if (message == null) return BadRequest("this message isnt available");
            if (message.SenderUsername != userName && message.RecipientUsername != userName) return Forbid();
            if(message.SenderUsername==userName) message.SenderDeleted = true;
            if (message.RecipientUsername==userName) message.RecipientDeleted = true;
            if (message.SenderDeleted == true || message.RecipientDeleted == true)
            {
                _messageRepository.DeleteMessage(message);
            }

            if (await _messageRepository.SaveAllAsync()) return Ok();
            return BadRequest("fail to delete message");

        }
    }
}
