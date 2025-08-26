using System.Linq.Expressions;
using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        // Basic CRUD
        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Query
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

        // Aggregates
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);

        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);

        Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Include Support (Eager Loading)
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);

        // Projection
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}