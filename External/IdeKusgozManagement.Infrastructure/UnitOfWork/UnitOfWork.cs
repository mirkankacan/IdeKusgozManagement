using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Infrastructure.Data.Context;
using IdeKusgozManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdeKusgozManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(type), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<IEnumerable<TResult>> ExecuteTableValuedFunctionAsync<TResult>(
           string functionName,
           object[] parameters,
           CancellationToken cancellationToken = default) where TResult : class
        {
            var parameterPlaceholders = string.Join(",", parameters.Select((_, i) => $"{{{i}}}"));
            var sql = $"SELECT * FROM {functionName}({parameterPlaceholders})";

            return await _context.Database
                .SqlQueryRaw<TResult>(sql, parameters)
                .ToListAsync(cancellationToken);
        }

        public async Task<TResult?> ExecuteScalarFunctionAsync<TResult>(
            string functionName,
            object[] parameters,
            CancellationToken cancellationToken = default)
        {
            var parameterPlaceholders = string.Join(",", parameters.Select((_, i) => $"{{{i}}}"));
            var sql = $"SELECT {functionName}({parameterPlaceholders})";

            return await _context.Database
                .SqlQueryRaw<TResult>(sql, parameters)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _transaction?.CommitAsync(cancellationToken);
            }
            catch
            {
                await _transaction?.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _transaction?.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context?.Dispose();
                _disposed = true;
            }
        }
    }
}