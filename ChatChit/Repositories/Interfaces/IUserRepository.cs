using ChatChit.Models;

namespace ChatChit.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetUserByName(string name);
        Task<User> GetUserByPhone(string phone);
        Task<IEnumerable<User>> GetUserNotInGroup(int groupId);
        Task<IEnumerable<User>> GetUserExceptMe(string userId);
    }
}
