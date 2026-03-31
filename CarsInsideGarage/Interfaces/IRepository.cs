using System.Linq.Expressions;

namespace CarsInsideGarage.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Remove(T entity); // Hard delete
        void SoftDelete (T entity); // Soft delete

        // For soft-deleted entities, this method will return null. For hard-deleted entities, it will throw an exception.
        Task<T?> GetByIdIncludingDeletedAsync(int id);

    }
}
