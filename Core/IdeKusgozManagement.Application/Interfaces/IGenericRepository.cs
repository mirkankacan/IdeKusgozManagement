using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Application.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    }
}
