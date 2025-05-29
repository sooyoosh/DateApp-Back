﻿using AutoMapper;
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
            CreateMap<Message, MessageDto>()
           .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))  // عکس پروفایل فرستنده
           .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));  // عکس پروفایل فرستنده
            CreateMap<DateTime,DateTime>().ConvertUsing(d=>DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?,DateTime?>().ConvertUsing(d=> d.HasValue?DateTime.SpecifyKind(d.Value, DateTimeKind.Utc):null);    
        }
    }
}
