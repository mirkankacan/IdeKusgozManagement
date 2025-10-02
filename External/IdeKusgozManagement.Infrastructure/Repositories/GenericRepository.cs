using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IdeKusgozManagement.Infrastructure.Repositories
{
    public class GenericRepository<T>(ApplicationDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
    {
        protected ApplicationDbContext Context = dbContext;
        private readonly DbSet<T> _dbSet = dbContext.Set<T>();

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "The entity parameter cannot be null.");

            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities), "The entities parameter cannot be null or empty.");

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public async Task<bool> AnyAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "The id parameter cannot be null or empty.");

            return await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }

        public ValueTask<T?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "The id parameter cannot be null or empty.");

            return _dbSet.FindAsync(id, cancellationToken);
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "The entity parameter cannot be null.");

            _dbSet.Remove(entity);
        }

        public async Task RemoveAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "The id parameter cannot be null or empty.");

            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new ArgumentNullException("Entity not found.", nameof(entity));

            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities), "The entities parameter cannot be null or empty.");

            _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "The entity parameter cannot be null.");

            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities), "The entities parameter cannot be null or empty.");

            _dbSet.UpdateRange(entities);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IQueryable<T> WhereAsNoTracking(Expression<Func<T, bool>>? predicate)
        {
            var query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        public async Task RemoveRangeByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids), "The ids parameter cannot be null or empty.");

            var entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
            _dbSet.RemoveRange(entities);
        }
    }
}