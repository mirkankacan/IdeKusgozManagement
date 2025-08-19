using System.Linq.Expressions;
using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        // Basic CRUD operations
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);

        // Query operations - SADECE Expression kullan!
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Count and existence operations
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Aggregate operations - Decimal
        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);

        Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default);

        Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Aggregate operations - Int
        Task<int> SumAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default);

        Task<int> SumAsync(Expression<Func<T, int>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int?> SumAsync(Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default);

        Task<int?> SumAsync(Expression<Func<T, int?>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Aggregate operations - Double
        Task<double> SumAsync(Expression<Func<T, double>> selector, CancellationToken cancellationToken = default);

        Task<double> SumAsync(Expression<Func<T, double>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<double?> SumAsync(Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default);

        Task<double?> SumAsync(Expression<Func<T, double?>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Average operations
        Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default);

        Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<double> AverageAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default);

        Task<double> AverageAsync(Expression<Func<T, int>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Min/Max operations
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        // Pagination operations
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetPagedAsync<TKey>(Expression<Func<T, TKey>> orderBy, int pageNumber, int pageSize, bool ascending = true, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetPagedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, int pageNumber, int pageSize, bool ascending = true, CancellationToken cancellationToken = default);

        // Include operations
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        // Bulk operations
        Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        // DateTime Range operations
        Task<IEnumerable<T>> GetByDateRangeAsync(Expression<Func<T, DateTime>> dateSelector, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetByCreatedDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        // Distinct and Group operations
        Task<IEnumerable<TResult>> SelectDistinctAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

        Task<IEnumerable<IGrouping<TKey, T>>> GroupByAsync<TKey>(Expression<Func<T, TKey>> keySelector, CancellationToken cancellationToken = default);
    }
}