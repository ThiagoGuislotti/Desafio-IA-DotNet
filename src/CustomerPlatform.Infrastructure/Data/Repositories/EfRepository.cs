using CustomerPlatform.Application.Abstractions.Paginations;
using CustomerPlatform.Application.Abstractions.Repositories;
using CustomerPlatform.Application.Abstractions.Results;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerPlatform.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repositorio generico baseado em EF Core.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
    public sealed class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        #region Variables
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dbContext">Contexto do EF Core.</param>
        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        /// <inheritdoc />
        public async Task<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(keyValues, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            return predicate is null
                ? await _dbSet.AnyAsync(cancellationToken).ConfigureAwait(false)
                : await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TEntity?> SearchFirstOrDefaultAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyPredicate(_dbSet.AsQueryable(), predicate);
            query = ApplyOrder(query, orderBy);
            return await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TSelector?> SearchFirstOrDefaultAsync<TSelector>(
            Expression<Func<TEntity, TSelector>> selector,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            bool enableQueryDistinct = false,
            Func<IQueryable<TSelector>, IOrderedQueryable<TSelector>>? orderBySelector = null,
            CancellationToken cancellationToken = default)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var query = ApplyPredicate(_dbSet.AsQueryable(), predicate)
                .Select(selector);

            if (enableQueryDistinct)
                query = query.Distinct();

            query = ApplyOrder(query, orderBySelector);
            return await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TEntity> SearchEnumerableAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = ApplyPredicate(_dbSet.AsQueryable(), predicate);
            query = ApplyOrder(query, orderBy);

            await foreach (var item in query.AsAsyncEnumerable())
                yield return item;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TSelector> SearchEnumerableAsync<TSelector>(
            Expression<Func<TEntity, TSelector>> selector,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            bool enableQueryDistinct = false,
            Func<IQueryable<TSelector>, IOrderedQueryable<TSelector>>? orderBySelector = null)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            var query = ApplyPredicate(_dbSet.AsQueryable(), predicate)
                .Select(selector);

            if (enableQueryDistinct)
                query = query.Distinct();

            query = ApplyOrder(query, orderBySelector);

            await foreach (var item in query.AsAsyncEnumerable())
                yield return item;
        }

        /// <inheritdoc />
        public async Task<PagedList<TEntity>> SearchPagedListAsync(
            Pagination? pagination = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            pagination ??= new Pagination();
            var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
            var pageSize = pagination.PageSize <= 0 ? 20 : pagination.PageSize;

            var query = ApplyPredicate(_dbSet.AsQueryable(), predicate);
            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PagedList<TEntity>(items, totalCount, pageNumber, pageSize);
        }

        /// <inheritdoc />
        public async Task<PagedList<TSelector>> SearchPagedListAsync<TSelector>(
            Expression<Func<TEntity, TSelector>> selector,
            Pagination? pagination = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            bool enableQueryDistinct = false,
            Func<IQueryable<TSelector>, IOrderedQueryable<TSelector>>? orderBySelector = null,
            CancellationToken cancellationToken = default)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            pagination ??= new Pagination();
            var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
            var pageSize = pagination.PageSize <= 0 ? 20 : pagination.PageSize;

            var query = ApplyPredicate(_dbSet.AsQueryable(), predicate)
                .Select(selector);

            if (enableQueryDistinct)
                query = query.Distinct();

            query = ApplyOrder(query, orderBySelector);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new PagedList<TSelector>(items, totalCount, pageNumber, pageSize);
        }

        /// <inheritdoc />
        public async Task<Result> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var entry = await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            return entry.State == EntityState.Added
                ? Result.Success()
                : Result.Failure("Falha ao inserir entidade.");
        }

        /// <inheritdoc />
        public Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            DetachIfTracked(entity);
            var entry = _dbSet.Update(entity);
            return Task.FromResult(
                entry.State == EntityState.Modified
                    ? Result.Success()
                    : Result.Failure("Falha ao atualizar entidade."));
        }

        /// <inheritdoc />
        public async Task<Result> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(keyValues, cancellationToken).ConfigureAwait(false);
            if (entity is null)
                return Result.Failure("Entidade nao encontrada.");

            var entry = _dbSet.Remove(entity);
            return entry.State == EntityState.Deleted
                ? Result.Success()
                : Result.Failure("Falha ao remover entidade.");
        }
        #endregion

        #region Private Methods/Operators
        private static IQueryable<T> ApplyPredicate<T>(
            IQueryable<T> query,
            Expression<Func<T, bool>>? predicate)
        {
            return predicate is null ? query : query.Where(predicate);
        }

        private static IQueryable<T> ApplyOrder<T>(
            IQueryable<T> query,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy)
        {
            return orderBy is null ? query : orderBy(query);
        }

        private void DetachIfTracked(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries<TEntity>()
                .FirstOrDefault(tracked => ReferenceEquals(tracked.Entity, entity));

            if (entry is not null)
                entry.State = EntityState.Detached;
        }
        #endregion
    }
}
