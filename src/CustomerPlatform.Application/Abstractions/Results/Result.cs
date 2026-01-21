using CustomerPlatform.Application.Abstractions.Validation;

namespace CustomerPlatform.Application.Abstractions.Results
{
    /// <summary>
    /// Resultado padrao para operacoes da aplicacao.
    /// </summary>
    public class Result
    {
        #region Public Properties
        /// <summary>
        /// Indica se a operacao foi bem-sucedida.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Mensagem associada ao resultado.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Erros da operacao.
        /// </summary>
        public IReadOnlyCollection<ValidationError> Errors { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="isSuccess">Indica sucesso.</param>
        /// <param name="errors">Erros associados.</param>
        /// <param name="message">Mensagem do resultado.</param>
        protected Result(bool isSuccess, IReadOnlyCollection<ValidationError> errors, string? message)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
            Message = message;
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria um resultado de sucesso.
        /// </summary>
        /// <param name="message">Mensagem opcional.</param>
        /// <returns>Resultado de sucesso.</returns>
        public static Result Success(string? message = null)
        {
            return new Result(true, Array.Empty<ValidationError>(), message);
        }

        /// <summary>
        /// Cria um resultado de falha por validacao.
        /// </summary>
        /// <param name="validationResult">Resultado de validacao.</param>
        /// <param name="message">Mensagem opcional.</param>
        /// <returns>Resultado de falha.</returns>
        public static Result Failure(ValidationResult validationResult, string? message = null)
        {
            if (validationResult is null)
                throw new ArgumentNullException(nameof(validationResult));

            return new Result(false, validationResult.Errors, message);
        }

        /// <summary>
        /// Cria um resultado de falha com mensagem simples.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resultado de falha.</returns>
        public static Result Failure(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Mensagem obrigatoria.", nameof(message));

            var errors = new[] { new ValidationError("General", message) };
            return new Result(false, errors, message);
        }

        /// <summary>
        /// Cria um resultado de falha com lista de erros.
        /// </summary>
        /// <param name="errors">Erros da operacao.</param>
        /// <param name="message">Mensagem opcional.</param>
        /// <returns>Resultado de falha.</returns>
        public static Result Failure(IEnumerable<ValidationError> errors, string? message = null)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            return new Result(false, errors.ToArray(), message);
        }
        #endregion
    }

    /// <summary>
    /// Resultado padrao para operacoes da aplicacao com retorno de dados.
    /// </summary>
    /// <typeparam name="T">Tipo do retorno.</typeparam>
    public sealed class Result<T> : Result
    {
        #region Public Properties
        /// <summary>
        /// Dados retornados pela operacao.
        /// </summary>
        public T? Data { get; }
        #endregion

        #region Constructors
        private Result(bool isSuccess, T? data, IReadOnlyCollection<ValidationError> errors, string? message)
            : base(isSuccess, errors, message)
        {
            Data = data;
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria um resultado de sucesso.
        /// </summary>
        /// <param name="data">Dados retornados.</param>
        /// <param name="message">Mensagem opcional.</param>
        /// <returns>Resultado de sucesso.</returns>
        public static Result<T> Success(T data, string? message = null)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            return new Result<T>(true, data, Array.Empty<ValidationError>(), message);
        }

        /// <summary>
        /// Cria um resultado de falha por validacao.
        /// </summary>
        /// <param name="validationResult">Resultado de validacao.</param>
        /// <param name="message">Mensagem opcional.</param>
        /// <returns>Resultado de falha.</returns>
        public static new Result<T> Failure(ValidationResult validationResult, string? message = null)
        {
            if (validationResult is null)
                throw new ArgumentNullException(nameof(validationResult));

            return new Result<T>(false, default, validationResult.Errors, message);
        }

        /// <summary>
        /// Cria um resultado de falha com mensagem simples.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resultado de falha.</returns>
        public static new Result<T> Failure(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Mensagem obrigatoria.", nameof(message));

            var errors = new[] { new ValidationError("General", message) };
            return new Result<T>(false, default, errors, message);
        }

        /// <summary>
        /// Cria um resultado de falha com lista de erros.
        /// </summary>
        /// <param name="errors">Erros da operacao.</param>
        /// <param name="message">Mensagem opcional.</param>
        /// <returns>Resultado de falha.</returns>
        public static new Result<T> Failure(IEnumerable<ValidationError> errors, string? message = null)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            return new Result<T>(false, default, errors.ToArray(), message);
        }
        #endregion
    }
}
