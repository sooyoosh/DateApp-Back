using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;

namespace DatingApp.Interfaces
{
    public interface ILikesRepository
    {

        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<PagedList<MemberDto>> GetUserLikes(Likesparams likesparams);
        Task<IEnumerable<int>> GetCurrentUserLikeIda(int currentUserId);
        void DeleteLike(UserLike like);
        void AddLike(UserLike like);
        //Task<bool> SaveChanges();
    }
}
