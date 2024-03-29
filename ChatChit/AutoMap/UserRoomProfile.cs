using AutoMapper;

namespace ChatChit.AutoMap
{
    public class UserRoomProfile : Profile
    {
        public UserRoomProfile()
        {
            CreateMap<Models.UserRoom, ViewModel.UserRoomViewModel>()
                //.ForMember(dst => dst.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(x => x.UserId))
                .ForMember(dst => dst.UserName, opt => opt.MapFrom(x => x.User.UserName))
                .ForMember(dst => dst.DisplayName, opt => opt.MapFrom(x => x.User.DisplayName))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.User.Avatar))
                .ForMember(dst => dst.Role, opt => opt.MapFrom(x => x.Role))
                .ForMember(dst => dst.RoomId, opt => opt.MapFrom(x => x.RoomId))
                .ForMember(dst => dst.RoomName, opt => opt.MapFrom(x => x.Room.RoomName));

            CreateMap<ViewModel.UserRoomViewModel, Models.UserRoom>();
        }
    }
}
