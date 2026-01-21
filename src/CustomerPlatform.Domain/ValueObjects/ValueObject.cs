using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerPlatform.Domain.ValueObjects
{
    /// <summary>
    /// Base para value objects.
    /// </summary>
    public abstract class ValueObject
    {
        #region Public Methods/Operators
        /// <summary>
        /// Determina se o objeto atual e igual ao objeto especificado.
        /// </summary>
        /// <param name="obj">Objeto para comparacao.</param>
        /// <returns><c>true</c> se forem iguais; caso contrario, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Retorna o hash do objeto.
        /// </summary>
        /// <returns>Hash calculado.</returns>
        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var component in GetEqualityComponents())
            {
                hash.Add(component);
            }

            return hash.ToHashCode();
        }

        /// <summary>
        /// Compara dois value objects.
        /// </summary>
        /// <param name="left">Objeto a esquerda.</param>
        /// <param name="right">Objeto a direita.</param>
        /// <returns><c>true</c> se forem iguais; caso contrario, <c>false</c>.</returns>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compara dois value objects.
        /// </summary>
        /// <param name="left">Objeto a esquerda.</param>
        /// <param name="right">Objeto a direita.</param>
        /// <returns><c>true</c> se forem diferentes; caso contrario, <c>false</c>.</returns>
        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Protected Methods/Operators
        /// <summary>
        /// Retorna os componentes de igualdade do value object.
        /// </summary>
        /// <returns>Componentes de igualdade.</returns>
        protected abstract IEnumerable<object?> GetEqualityComponents();
        #endregion
    }
}
