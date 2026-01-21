using CustomerPlatform.Application.Abstractions;
using CustomerPlatform.Application.Abstractions.Repositories;
using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerPlatform.Infrastructure.Data.UnitOfWork
{
    /// <summary>
    /// Unidade de trabalho para EF Core.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        #region Variables
        private readonly CustomerPlatformDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new();
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dbContext">DbContext da aplicacao.</param>
        public UnitOfWork(CustomerPlatformDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);
            if (_repositories.TryGetValue(type, out var repository))
                return (IRepository<TEntity>)repository;

            var created = new EfRepository<TEntity>(_dbContext);
            _repositories[type] = created;
            return created;
        }

        /// <inheritdoc />
        public async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var strategy = _dbContext.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _dbContext.Database
                        .BeginTransactionAsync(cancellationToken)
                        .ConfigureAwait(false);

                    try
                    {
                        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                        return Result.Success();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                        return Result.Failure($"Erro ao persistir dados: {ex.Message}");
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Erro ao iniciar transacao: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_dbContext.Database.CurrentTransaction is not null)
                await _dbContext.Database.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);

            _dbContext.ChangeTracker.Clear();
        }
        #endregion
    }
}