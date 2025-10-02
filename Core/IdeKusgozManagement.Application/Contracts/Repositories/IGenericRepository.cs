using IdeKusgozManagement.Domain.Entities.Base;
using System.Linq.Expressions;

namespace IdeKusgozManagement.Application.Interfaces.UnitOfWork
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<bool> AnyAsync(string id, CancellationToken cancellationToken);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        ValueTask<T?> GetByIdAsync(string id, CancellationToken cancellationToken);

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        IQueryable<T> WhereAsNoTracking(Expression<Func<T, bool>>? predicate);

        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        void Update(T entity);

        void UpdateRange(IEnumerable<T> entities);

        Task RemoveAsync(string id, CancellationToken cancellationToken);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        Task RemoveRangeByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken);
    }
}