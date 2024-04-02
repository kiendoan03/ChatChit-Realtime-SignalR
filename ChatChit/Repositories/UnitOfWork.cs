using ChatChit.DAL;
using ChatChit.Repositories.Interfaces;
using ChatChit.Services.Interfaces;

namespace ChatChit.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;

        public UnitOfWork(ChatDbContext context)
        {
            _context = context;
            UserRepository = new UserRepository(_context);
        }
        public IUserRepository UserRepository { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
