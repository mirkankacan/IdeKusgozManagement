using IdeKusgozManagement.Domain.Entities.Base;

namespace IdeKusgozManagement.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : BaseEntity;

        Task<IEnumerable<TResult>> ExecuteTableValuedFunctionAsync<TResult>(
         string functionName,
         object[] parameters,
         CancellationToken cancellationToken) where TResult : class;

        Task<TResult?> ExecuteScalarFunctionAsync<TResult>(
         string functionName,
         object[] parameters,
         CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}