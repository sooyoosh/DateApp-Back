using DatingApp.Interfaces;

namespace DatingApp.Data
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILikesRepository _ikesRepository;
        public UnitOfWork(DataContext context,IUserRepository userRepository,IMessageRepository messageRepository,ILikesRepository likesRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _ikesRepository = likesRepository;
        }

        public IUserRepository UserRepository => UserRepository;

        public ILikesRepository LikesRepository => LikesRepository;

        public IMessageRepository MessageRepository => MessageRepository;

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() >0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
