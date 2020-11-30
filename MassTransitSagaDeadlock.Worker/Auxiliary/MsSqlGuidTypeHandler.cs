using System;
using System.Data;
using Dapper;

namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    /// <summary>
    /// Guid handler
    /// </summary>
    /// <seealso cref="Dapper.SqlMapper.TypeHandler{System.Guid}" />
    public sealed class MsSqlGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="guid">The unique identifier.</param>
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        /// <summary>
        /// Parse a database value back to a typed value
        /// </summary>
        /// <param name="value">The value from the database</param>
        /// <returns>The GUID</returns>
        public override Guid Parse(object value)
        {
            if (value == null || string.IsNullOrEmpty(value as string))
                return Guid.Empty;

            return new Guid((string)value);
        }
    }
}
