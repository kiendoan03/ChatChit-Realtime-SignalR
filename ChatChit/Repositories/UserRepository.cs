using ChatChit.DAL;
using ChatChit.Models;
using ChatChit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatChit.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ChatDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetUserByName(string name)
        {
            return await _context.Users.Where(u => u.DisplayName == name).ToListAsync();
        }

        public async Task<User> GetUserByPhone(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        }

        public async Task<IEnumerable<User>> GetUserExceptMe(string userId)
        {
            return await _context.Users.Where(u => u.Id != userId).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUserNotInGroup(int groupId)
        {
            var group = await _context.Rooms.Include(g => g.UserRoom).FirstOrDefaultAsync(g => g.Id == groupId);  
            var users = await _context.Users.ToListAsync();
            var usersInGroup = group.UserRoom.Select(ur => ur.UserId).ToList();
            return users.Where(u => !usersInGroup.Contains(u.Id));
        }
    }
}
