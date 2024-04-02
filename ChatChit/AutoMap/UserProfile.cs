using AutoMapper;
using ChatChit.Models;
using ChatChit.ViewModel;

namespace ChatChit.AutoMap
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //CreateMap<User, UserViewModel>();
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            //.ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));

            CreateMap<User, UserViewModel>().ReverseMap();
        }
    }
}
