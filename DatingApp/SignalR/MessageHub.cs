using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    public class MessageHub(IMessageRepository messageRepository,IUserRepository userRepository,IMapper mapper):Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext=Context.GetHttpContext();
            if (Context.User == null) throw new HubException("cannot get current user claim");
            var userName = Context.User.GetUsername();

            var otherUser = httpContext.Request.Query["user"].ToString();
            if (string.IsNullOrEmpty(otherUser))
                throw new HubException("Missing 'user' in query string");
            var groupName = GetGroupName(userName, otherUser);
            if (Context.User == null || string.IsNullOrEmpty(otherUser)) throw new Exception("cannot join group");
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
            var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {


            await base.OnDisconnectedAsync(exception);
        }


            public async Task SendMessage(CreateMessageDto createMessage)
            {
                var username = Context.User.GetUsername()?? throw new Exception("could not found user");

                if (username == createMessage.RecipientUserName.ToLower())
                    throw new HubException("You cannot send messages to yourself");

                var sender = await userRepository.GetUserByUsernameAsync(username);
                var recipient = await userRepository.GetUserByUsernameAsync(createMessage.RecipientUserName);




                var message = new Message
                {
                    Sender = sender,
                    SenderUsername = username,
                    Recipient = recipient,
                    RecipientUsername = createMessage.RecipientUserName,
                    Content = createMessage.Content
                };
                messageRepository.AddMessage(message);

                if (await messageRepository.SaveAllAsync())
                {
                    var groupName = GetGroupName(sender.UserName,recipient.UserName);
                    await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
                    return;
                }

                throw new HubException("Failed to send message");
            }


        private string GetGroupName(string caller,string other)
        {
            var stringCompare=string.CompareOrdinal(caller,other)<0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
