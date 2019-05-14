using AutoMapper;
using DatingAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.DTOs.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>()
                    .ForMember(dest => dest.PhotoURL, opt => opt.MapFrom(src => src.Photos.Where(p => p.IsMain)
                    .Select(x => x.Url).FirstOrDefault()))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Now.Year - src.DateOfBirth.Year));
            CreateMap<UserDTO, User>();

            CreateMap<User, UserDetailDTO>()
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => DateTime.Now.Year - src.DateOfBirth.Year))
                    .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.Where(p => p.IsMain)
                    .Select(x => x.Url).FirstOrDefault()));
            CreateMap<UserDetailDTO, User>();

            //CreateMap<IEnumerable<User>, IEnumerab le<UserDTO>>();
            CreateMap<Photo, PhotoDTO>();
            CreateMap<PhotoDTO, Photo>();

            CreateMap<Photo, PhotoForReturnDTO>();


            CreateMap<Message, MessageToReturnDTO>()
                .ForMember(dest => dest.RecipientKnownAs, opt => opt.MapFrom(src => src.Recipient.KnownAs))
                .ForMember(dest => dest.SenderKnownAs, opt => opt.MapFrom(src => src.Sender.KnownAs))
                .ForMember(dest => dest.RecipientPhotoURL, opt => opt.MapFrom(src => src.Recipient.Photos.Where(p => p.IsMain)
                    .Select(x => x.Url).FirstOrDefault()))
                .ForMember(dest => dest.SenderPhotoURL, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain == true).Url));




            CreateMap<Message, MessageInOutBoxDTO>()
                   .ForMember(dest => dest.RecipientKnownAs, opt => opt.MapFrom(src => src.Recipient.KnownAs))
                   .ForMember(dest => dest.SenderKnownAs, opt => opt.MapFrom(src => src.Sender.KnownAs))
                   .ForMember(dest => dest.RecipientPhotoURL, opt => opt.MapFrom(src => src.Recipient.Photos.Where(p => p.IsMain)
                        .Select(x => x.Url).FirstOrDefault()))
                   .ForMember(dest => dest.SenderPhotoURL, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain == true).Url));
        }
    }
}
