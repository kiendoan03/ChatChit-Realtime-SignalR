using ChatChit.DAL;
using ChatChit.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatChit.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly ChatDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ChatDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task Delete(T entity)
        {
            await Task.Run(() => _dbSet.Remove(entity));
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Update(T entity)
        {
             _dbSet.Attach(entity);
             _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
