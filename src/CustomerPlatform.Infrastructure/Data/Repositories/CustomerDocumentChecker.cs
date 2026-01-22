using CustomerPlatform.Application.Abstractions.Repositories;
using CustomerPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CustomerPlatform.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Verifica duplicidade de documentos no banco.
    /// </summary>
    public sealed class CustomerDocumentChecker : ICustomerDocumentChecker
    {
        #region Variables
        private readonly CustomerPlatformDbContext _dbContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dbContext">Contexto do banco.</param>
        public CustomerDocumentChecker(CustomerPlatformDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public async Task<bool> ExistsAsync(string document, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(document))
                return false;

            var connection = _dbContext.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
                await _dbContext.Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM \"Customers\" WHERE \"document\" = @document LIMIT 1";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "document";
                parameter.Value = document;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                return result is not null;
            }
            finally
            {
                if (shouldClose)
                    await _dbContext.Database.CloseConnectionAsync().ConfigureAwait(false);
            }
        }
        #endregion
    }
}