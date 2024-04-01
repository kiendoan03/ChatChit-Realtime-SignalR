using AutoMapper;

namespace ChatChit.AutoMap
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Models.Room, ViewModel.RoomViewModel>()
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoomId))
                //.ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName))
                .ForMember(dest => dest.Admin, opt => opt.MapFrom(src => src.Admin.Id));

            CreateMap<ViewModel.RoomViewModel, Models.Room>();
        }
    }
}
