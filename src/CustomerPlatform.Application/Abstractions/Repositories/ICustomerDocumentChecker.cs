#if !IMPLICIT_USINGS
using System.Threading;
using System.Threading.Tasks;
#endif

namespace CustomerPlatform.Application.Abstractions.Repositories
{
    /// <summary>
    /// Contrato para verificar duplicidade de documento.
    /// </summary>
    public interface ICustomerDocumentChecker
    {
        #region Public Methods/Operators
        /// <summary>
        /// Verifica se existe cliente com o documento informado.
        /// </summary>
        /// <param name="document">Documento normalizado.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><c>true</c> quando existir.</returns>
        Task<bool> ExistsAsync(string document, CancellationToken cancellationToken = default);
        #endregion
    }
}
