using AutoMapper;

namespace ChatChit.AutoMap
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.User, ViewModel.UserViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));

            CreateMap<ViewModel.UserViewModel, Models.User>();
        }
    }
}
