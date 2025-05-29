using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class MessageRepository : IMessageRepository
    {


        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await _context.Connections.FirstAsync(x=>x.ConnectionId == connectionId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return  await _context.Groups
                .Include(x=>x.Connections)
                .FirstOrDefaultAsync(x=>x.Name== groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query=_context.Messages
                .OrderByDescending(x=>x.MessageSent)
                .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username),
                "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username),
                _ => query.Where(m => m.Recipient.UserName == messageParams.Username && m.DateRead == null)
            };

            var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
               


            return await PagedList<MessageDto>.CreateAsync(
                message, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            
         var messages = _context.Messages
        .Include(x => x.Sender).ThenInclude(x => x.Photos)
        .Include(x => x.Recipient).ThenInclude(x => x.Photos)
        .Where(m => (m.RecipientUsername == currentUsername && m.SenderUsername == recipientUsername) ||
                    (m.SenderUsername == currentUsername && m.RecipientUsername == recipientUsername))
        .OrderBy(m => m.MessageSent)
        .ProjectTo<MessageDto>(_mapper.ConfigurationProvider) // 
        .AsQueryable();

            var messageList =  messages.ToList();

            // Handle unread messages
            var unreadMessages = messageList.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();
            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
                await _context.SaveChangesAsync();
            }

            return messageList;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
