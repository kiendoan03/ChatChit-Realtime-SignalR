using ChatChit.Models;
using ChatChit.ViewModel;

namespace ChatChit.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAllUser();
        Task<UserViewModel> GetUserById(string id);
        Task<IEnumerable<UserViewModel>> GetUserByName(string name);
        Task<UserViewModel> GetUserByPhone(string phone);
        Task EditUser(UserViewModel user);
        Task <IEnumerable<UserViewModel>> GetUserNotInGroup(int groupId);
        Task<IEnumerable<UserViewModel>> GetUserExceptMe(string userId);
    }
}
