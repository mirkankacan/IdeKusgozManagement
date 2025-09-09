using System.Linq.Expressions;
using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Interfaces.Repositories;
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

        /// <summary>
        /// Belirtilen ID'ye sahip entity'yi tracking ile getirir. Update/Delete işlemleri için kullanılır.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        /// <summary>
        /// Belirtilen ID'ye sahip entity'yi AsNoTracking ile getirir. Read-only işlemler için performanslıdır.
        /// </summary>
        public virtual async Task<T?> GetByIdNoTrackingAsync(string id, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        /// <summary>
        /// Tüm entity'leri tracking ile getirir. Update/Delete işlemleri için kullanılır.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.ToListAsync(cancellationToken);

        /// <summary>
        /// Tüm entity'leri AsNoTracking ile getirir. Read-only işlemler için performanslıdır.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllNoTrackingAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

        /// <summary>
        /// Yeni bir entity ekler ve eklenen entity'yi döndürür.
        /// </summary>
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        /// <summary>
        /// Birden fazla entity'yi toplu olarak ekler ve eklenen entity'leri döndürür.
        /// </summary>
        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        /// <summary>
        /// Mevcut bir entity'yi günceller ve güncellenen entity'yi döndürür.
        /// </summary>
        public virtual Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Birden fazla entity'yi toplu olarak günceller ve güncellenen entity'leri döndürür.
        /// </summary>
        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (!entities.Any()) return Enumerable.Empty<T>();
            _dbSet.UpdateRange(entities);
            return await Task.FromResult(entities);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip entity'yi siler. Silme işlemi başarılı olursa true döndürür.
        /// </summary>
        public virtual async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity is null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        /// <summary>
        /// Verilen entity'yi siler. Silme işlemi başarılı olursa true döndürür.
        /// </summary>
        public virtual Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) return Task.FromResult(false);

            _dbSet.Remove(entity);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Verilen entity koleksiyonunu toplu olarak siler. Silme işlemi başarılı olursa true döndürür.
        /// </summary>
        public virtual Task<bool> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            if (!entities.Any()) return Task.FromResult(false);

            _dbSet.RemoveRange(entities);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Belirtilen koşula uyan tüm entity'leri siler. Silme işlemi başarılı olursa true döndürür.
        /// </summary>
        public virtual async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            if (!entities.Any()) return false;

            _dbSet.RemoveRange(entities);
            return true;
        }

        // ---------------- Query ----------------

        /// <summary>
        /// Belirtilen koşula uyan entity'leri tracking ile getirir. Update/Delete işlemleri için kullanılır.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity'leri AsNoTracking ile getirir. Read-only işlemler için performanslıdır.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetWhereNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan ilk entity'yi tracking ile getirir. Update/Delete işlemleri için kullanılır.
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan ilk entity'yi AsNoTracking ile getirir. Read-only işlemler için performanslıdır.
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan tek entity'yi tracking ile getirir. Birden fazla sonuç varsa exception fırlatır.
        /// </summary>
        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan tek entity'yi AsNoTracking ile getirir. Birden fazla sonuç varsa exception fırlatır.
        /// </summary>
        public virtual async Task<T?> SingleOrDefaultNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken);

        /// <summary>
        /// Herhangi bir entity var mı kontrol eder. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(cancellationToken);

        /// <summary>
        /// Herhangi bir entity var mı kontrol eder. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<bool> AnyNoTrackingAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().AnyAsync(cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan herhangi bir entity var mı kontrol eder. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan herhangi bir entity var mı kontrol eder. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<bool> AnyNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen ID'ye sahip entity var mı kontrol eder. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);

        /// <summary>
        /// Belirtilen ID'ye sahip entity var mı kontrol eder. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<bool> ExistsNoTrackingAsync(string id, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken);

        // ---------------- Aggregates ----------------

        /// <summary>
        /// Toplam entity sayısını döndürür. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(cancellationToken);

        /// <summary>
        /// Toplam entity sayısını döndürür. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<int> CountNoTrackingAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().CountAsync(cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity sayısını döndürür. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity sayısını döndürür. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<int> CountNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().CountAsync(predicate, cancellationToken);

        /// <summary>
        /// Belirtilen property'nin toplam değerini döndürür. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => await _dbSet.SumAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen property'nin toplam değerini döndürür. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<decimal> SumNoTrackingAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().SumAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity'lerin property toplamını döndürür. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).SumAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity'lerin property toplamını döndürür. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<decimal> SumNoTrackingAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(predicate).SumAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen property'nin ortalama değerini döndürür. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => await _dbSet.AverageAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen property'nin ortalama değerini döndürür. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<decimal> AverageNoTrackingAsync(Expression<Func<T, decimal>> selector, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().AverageAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity'lerin property ortalamasını döndürür. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).AverageAsync(selector, cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity'lerin property ortalamasını döndürür. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<decimal> AverageNoTrackingAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(predicate).AverageAsync(selector, cancellationToken);

        // ---------------- Includes ----------------

        /// <summary>
        /// Tüm entity'leri belirtilen navigation property'ler ile birlikte tracking ile getirir.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = ApplyIncludes(query, includeProperties);
            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Tüm entity'leri belirtilen navigation property'ler ile birlikte AsNoTracking ile getirir.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllNoTrackingAsync(CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            query = ApplyIncludes(query, includeProperties);
            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip entity'yi navigation property'ler ile birlikte tracking ile getirir.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = ApplyIncludes(query, includeProperties);
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip entity'yi navigation property'ler ile birlikte AsNoTracking ile getirir.
        /// </summary>
        public virtual async Task<T?> GetByIdNoTrackingAsync(string id, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            query = ApplyIncludes(query, includeProperties);
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        /// <summary>
        /// Belirtilen koşula uyan entity'leri navigation property'ler ile birlikte tracking ile getirir.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            query = ApplyIncludes(query, includeProperties);
            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen koşula uyan entity'leri navigation property'ler ile birlikte AsNoTracking ile getirir.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetWhereNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate);
            query = ApplyIncludes(query, includeProperties);
            return await query.ToListAsync(cancellationToken);
        }

        // ---------------- Projection ----------------

        /// <summary>
        /// Belirtilen koşula uyan entity'leri belirtilen selector ile projeksiyon yapar. Tracking ile çalışır.
        /// </summary>
        public virtual async Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).Select(selector).ToListAsync(cancellationToken);

        /// <summary>
        /// Belirtilen koşula uyan entity'leri belirtilen selector ile projeksiyon yapar. AsNoTracking ile performanslıdır.
        /// </summary>
        public virtual async Task<IEnumerable<TResult>> SelectNoTrackingAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().Where(predicate).Select(selector).ToListAsync(cancellationToken);

        // ---------------- Private Helper ----------------

        /// <summary>
        /// Verilen query'ye belirtilen navigation property'leri include eder.
        /// </summary>
        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties)
        {
            foreach (var include in includeProperties)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.AsQueryable();

            // Include properties
            if (includeProperties?.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            // Total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Calculate pagination
            var skip = (pageNumber - 1) * pageSize;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Get paged data
            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResult<T>> GetPagedNoTrackingAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            // Include properties
            if (includeProperties?.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            // Total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Calculate pagination
            var skip = (pageNumber - 1) * pageSize;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Get paged data
            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
    }
}