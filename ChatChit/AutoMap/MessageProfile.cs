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
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.FromUser.DisplayName))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.ToRoom.RoomName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.FromUser.Avatar))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content)));

            CreateMap<MessageViewModel, Message>();
        }
    }
}
