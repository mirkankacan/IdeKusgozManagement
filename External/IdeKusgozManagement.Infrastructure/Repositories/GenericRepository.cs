using System.Linq.Expressions;
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

        // ---------------- Basic CRUD ----------------
        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.ToListAsync(cancellationToken);

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (!entities.Any()) return Enumerable.Empty<T>();
            _dbSet.UpdateRange(entities);
            return await Task.FromResult(entities);
        }

        public virtual async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity is null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        public virtual Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) return Task.FromResult(false);

            _dbSet.Remove(entity);
            return Task.FromResult(true);
        }

        public virtual Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (!entities.Any()) return Task.FromResult(false);

            _dbSet.RemoveRange(entities);
            return Task.FromResult(true);
        }

        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            if (!entities.Any()) return false;

            _dbSet.RemoveRange(entities);
            return true;
        }

        // ---------------- Query ----------------
        public virtual async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);

        public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(cancellationToken);

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);

        // ---------------- Aggregates ----------------
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(cancellationToken);

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(predicate, cancellationToken);

        public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => await _dbSet.SumAsync(selector, cancellationToken);

        public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);

        public virtual async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => await _dbSet.AverageAsync(selector, cancellationToken);

        public virtual async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).AverageAsync(selector, cancellationToken);

        // ---------------- Includes ----------------
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = ApplyIncludes(query, includeProperties);
            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = ApplyIncludes(query, includeProperties);
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            query = ApplyIncludes(query, includeProperties);
            return await query.ToListAsync(cancellationToken);
        }

        // ---------------- Projection ----------------
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).Select(selector).ToListAsync(cancellationToken);

        // ---------------- Private Helper ----------------
        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
        {
            foreach (var include in includeProperties)
            {
                query = query.Include(include);
            }
            return query;
        }
    }
}