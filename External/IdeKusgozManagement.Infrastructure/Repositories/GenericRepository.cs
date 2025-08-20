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

        #region Mevcut Metodlar

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

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                return false;

            _dbSet.RemoveRange(entities);
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

        #endregion Mevcut Metodlar

        #region Expression Tabanlý Metodlar

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion Expression Tabanlý Metodlar

        #region Conditional Metodlar

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AllAsync(predicate, cancellationToken);
        }

        #endregion Conditional Metodlar

        #region Sum Metodlarý - Decimal

        public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);
        }

        public virtual async Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);
        }

        #endregion Sum Metodlarý - Decimal

        #region Sum Metodlarý - Int

        public virtual async Task<int> SumAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<int> SumAsync(Expression<Func<T, int>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);
        }

        public virtual async Task<int?> SumAsync(Expression<Func<T, int?>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<int?> SumAsync(Expression<Func<T, int?>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);
        }

        #endregion Sum Metodlarý - Int

        #region Sum Metodlarý - Double

        public virtual async Task<double> SumAsync(Expression<Func<T, double>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<double> SumAsync(Expression<Func<T, double>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);
        }

        public virtual async Task<double?> SumAsync(Expression<Func<T, double?>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SumAsync(selector, cancellationToken);
        }

        public virtual async Task<double?> SumAsync(Expression<Func<T, double?>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);
        }

        #endregion Sum Metodlarý - Double

        #region Average Metodlarý

        public virtual async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AverageAsync(selector, cancellationToken);
        }

        public virtual async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).AverageAsync(selector, cancellationToken);
        }

        public virtual async Task<double> AverageAsync(Expression<Func<T, int>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AverageAsync(selector, cancellationToken);
        }

        public virtual async Task<double> AverageAsync(Expression<Func<T, int>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).AverageAsync(selector, cancellationToken);
        }

        #endregion Average Metodlarý

        #region Min/Max Metodlarý

        public virtual async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.MinAsync(selector, cancellationToken);
        }

        public virtual async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).MinAsync(selector, cancellationToken);
        }

        public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.MaxAsync(selector, cancellationToken);
        }

        public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).MaxAsync(selector, cancellationToken);
        }

        #endregion Min/Max Metodlarý

        #region Pagination Metodlarý

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync<TKey>(Expression<Func<T, TKey>> orderBy, int pageNumber, int pageSize, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var query = ascending ? _dbSet.OrderBy(orderBy) : _dbSet.OrderByDescending(orderBy);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, int pageNumber, int pageSize, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(predicate);
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        #endregion Pagination Metodlarý

        #region Include Metodlarý

        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T?> GetByIdWithIncludesAsync(string id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        #endregion Include Metodlarý

        #region Bulk Operations

        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            if (!entities.Any())
                return false;

            _dbSet.RemoveRange(entities);
            return true;
        }

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
                return Enumerable.Empty<T>();

            _dbSet.UpdateRange(entities);
            return await Task.FromResult(entities);
        }

        #endregion Bulk Operations

        #region DateTime Range Metodlarý

        public virtual async Task<IEnumerable<T>> GetByDateRangeAsync(Expression<Func<T, DateTime>> dateSelector, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(BuildDateRangePredicate(dateSelector, startDate, endDate))
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetByCreatedDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.CreatedDate >= startDate && e.CreatedDate <= endDate)
                .ToListAsync(cancellationToken);
        }

        private Expression<Func<T, bool>> BuildDateRangePredicate(Expression<Func<T, DateTime>> dateSelector, DateTime startDate, DateTime endDate)
        {
            var parameter = dateSelector.Parameters[0];
            var dateProperty = dateSelector.Body;

            var startComparison = Expression.GreaterThanOrEqual(dateProperty, Expression.Constant(startDate));
            var endComparison = Expression.LessThanOrEqual(dateProperty, Expression.Constant(endDate));
            var combined = Expression.AndAlso(startComparison, endComparison);

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        #endregion DateTime Range Metodlarý

        #region Distinct ve Group By Metodlarý

        public virtual async Task<IEnumerable<TResult>> SelectDistinctAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Select(selector)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<IGrouping<TKey, T>>> GroupByAsync<TKey>(Expression<Func<T, TKey>> keySelector, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .GroupBy(keySelector)
                .ToListAsync(cancellationToken);
        }



        #endregion Distinct ve Group By Metodlarý
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                   .Where(predicate)
                   .Select(selector)
                   .ToListAsync(cancellationToken);
        }
    }
}