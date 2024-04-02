using AutoMapper;
using ChatChit.DAL;
using ChatChit.Hubs;
using ChatChit.Models;
using ChatChit.Repositories.Interfaces;
using ChatChit.Services.Interfaces;
using ChatChit.ViewModel;
using Microsoft.AspNetCore.SignalR;

namespace ChatChit.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public Task EditUser(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUser()
        {
            var users = await _unitOfWork.UserRepository.GetAll();
            return _mapper.Map<IEnumerable<UserViewModel>>(users);
        }

        public async Task<UserViewModel> GetUserById(string id)
        {
            return _mapper.Map<UserViewModel>(await _unitOfWork.UserRepository.GetById(id));
        }

        public async Task<IEnumerable<UserViewModel>> GetUserByName(string name)
        {
            var users = await _unitOfWork.UserRepository.GetUserByName(name);
            return _mapper.Map<IEnumerable<UserViewModel>>(users);
        }


        public async Task<UserViewModel> GetUserByPhone(string phone)
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhone(phone);
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<IEnumerable<UserViewModel>> GetUserNotInGroup(int groupId)
        {
            var users = await _unitOfWork.UserRepository.GetUserNotInGroup(groupId);
            return _mapper.Map<IEnumerable<UserViewModel>>(users);
        }
    }
}
