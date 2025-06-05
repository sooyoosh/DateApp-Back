using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DatingApp.Data
{
    public class LikesRepository: ILikesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public LikesRepository(DataContext context, IMapper mapper)    
        {
            _context = context;
            _mapper = mapper;

        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }
        public async Task<PagedList<MemberDto>> GetUserLikes(Likesparams likesparams)
        {
            var likes=_context.Likes.AsQueryable();
            //var users=_context.Users.AsQueryable();
            IQueryable<MemberDto> query;



            switch (likesparams.Predicate)
            {
                case "liked":
                    query = likes
                        .Where(like => like.SourceUserId == likesparams.UserId)
                        .Select(like => like.TargetUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;

                case "likedBy":
                    query = likes
                        .Where(like => like.TargetUserId == likesparams.UserId)
                        .Select(like => like.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;

                default:
                    var likeIds = await GetCurrentUserLikeIda(likesparams.UserId);

                    query = likes
                        .Where(x => x.TargetUserId == likesparams.UserId && likeIds.Contains(x.SourceUserId))
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;
            }


            return await PagedList<MemberDto>.CreateAsync(query, likesparams.PageNumber, likesparams.PageSize);
            

        }
        public async Task<IEnumerable<int>> GetCurrentUserLikeIda(int currentUserId)
        {
            return await _context.Likes.Where(x=>x.SourceUserId==currentUserId)
                .Select(x=>x.TargetUserId)
                .ToListAsync();
        }
        public void AddLike(UserLike like)
        {
            _context.Add(like);
        }

        public void DeleteLike(UserLike like)
        {
            _context.Remove(like);
        }




        //public async Task<bool> SaveChanges()
        //{
        //    return await _context.SaveChangesAsync()>0;
        //}
    }
}
