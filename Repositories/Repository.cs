using CarsInsideGarage.Data;
using CarsInsideGarage.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarsInsideGarage.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly GarageDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(GarageDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void SoftDelete(T entity)
        {
            if (entity is ISoftDeletable soft)
            {
                soft.IsDeleted = true;
                soft.DeletedAt = DateTime.UtcNow;
            }
        }

        public async Task<T?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _dbSet
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }


    }
}
