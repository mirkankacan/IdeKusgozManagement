using IdeKusgozManagement.Application.Interfaces;
using IdeKusgozManagement.Domain.Entities.Base;
using IdeKusgozManagement.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace IdeKusgozManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_dbSet.Where(predicate).ToList());
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public virtual async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return await Task.FromResult(true);
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}