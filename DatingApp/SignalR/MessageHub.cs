using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    public class MessageHub(IUnitOfWork unitOfWork,
        IMapper mapper,IHubContext<PresenceHub> presenceHub):Hub
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
            await AddToGroup(groupName);
            var messages = await unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }


            public async Task SendMessage(CreateMessageDto createMessage)
            {
                var username = Context.User.GetUsername()?? throw new Exception("could not found user");

                if (username == createMessage.RecipientUserName.ToLower())
                    throw new HubException("You cannot send messages to yourself");

                var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessage.RecipientUserName);




                var message = new Message
                {
                    Sender = sender,
                    SenderUsername = username,
                    Recipient = recipient,
                    RecipientUsername = createMessage.RecipientUserName,
                    Content = createMessage.Content
                };

                var groupName= GetGroupName(sender.UserName, recipient.UserName);
                var group= await unitOfWork.MessageRepository.GetMessageGroup(groupName);          

                if(group !=null && group.Connections.Any(x=>x.Username == recipient.UserName))
                {
                    message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections=await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections !=null && connections?.Count != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved", new
                    {
                        username = sender.UserName,
                        knownAs = sender.KnownAs,
                        content=createMessage.Content
                    });
                }
            }
                unitOfWork.MessageRepository.AddMessage(message);

                if (await unitOfWork.Complete())
                {
                    //var groupName = GetGroupName(sender.UserName,recipient.UserName);
                    await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
                    return;
                }

                throw new HubException("Failed to send message");
            }

        private async Task<bool> AddToGroup(string groupName)
        {
            var userName = Context.User.GetUsername() ?? throw new Exception("username not found");
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection { ConnectionId = Context.ConnectionId,Username=userName };
            if (group == null)
            {
                group=new Group { Name= groupName };
                unitOfWork.MessageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            return await unitOfWork.Complete();
        }


        private async Task RemoveFromMessageGroup()
        {
            var connection=await unitOfWork.MessageRepository.GetConnection(Context.ConnectionId);
            if (connection != null)
            {
                unitOfWork.MessageRepository.RemoveConnection(connection);
                await unitOfWork.Complete();
            }
        }



         


        private string GetGroupName(string caller,string other)
        {
            var stringCompare=string.CompareOrdinal(caller,other)<0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
