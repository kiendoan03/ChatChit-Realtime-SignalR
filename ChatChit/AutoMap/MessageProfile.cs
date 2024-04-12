using AutoMapper;
using ChatChit.Helpers;
using ChatChit.Models;
using ChatChit.ViewModel;

namespace ChatChit.AutoMap
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageViewModel>()
                .ForMember(dest => dest.FromUser, opt => opt.MapFrom(src => src.FromUser.DisplayName))
                .ForMember(dest => dest.ToUser, opt => opt.MapFrom(src => src.ToUser.DisplayName))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.ToRoom.RoomName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.FromUser.Avatar))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content)))
                .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => BasicEmojis.ParseEmojis(src.Parent.Content)))
                .ForMember(dest => dest.OwnerParent, opt => opt.MapFrom(src => src.Parent.FromUser.DisplayName));

            CreateMap<MessageViewModel, Message>();
        }
    }
}
