using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;

namespace DatingApp.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dist=>dist.Age,opt=>opt.MapFrom(src=>src.DateOfBirth.CalculateAgee()))
                .ForMember(dist=>dist.PhotoUrl,opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain)!.Url));
            CreateMap<Photo,PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<string,DateOnly>().ConvertUsing(s=>DateOnly.Parse(s));    
        }
    }
}
