using System;
using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers.Pagination;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(
                    src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(
                    src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url));

            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();

            CreateMap<UserParams, UserHeader>();
            CreateMap<LikesParams, LikesHeader>();
            CreateMap<MessageParams, MessageHeader>();

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        }
    }
}