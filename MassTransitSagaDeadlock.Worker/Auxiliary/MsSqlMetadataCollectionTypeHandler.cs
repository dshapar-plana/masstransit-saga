using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    /// <summary>
    /// Metadata collection handler
    /// </summary>
    /// <seealso cref="Dapper.SqlMapper.TypeHandler{System.Collections.Generic.Dictionary{System.String, System.Object}}" />
    public sealed class MsSqlMetadataCollectionTypeHandler : SqlMapper.TypeHandler<Dictionary<string, object>>
    {
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="metadata">The metadata.</param>
        public override void SetValue(IDbDataParameter parameter, Dictionary<string, object> metadata)
        {
            parameter.Value = JsonConvert.SerializeObject(metadata);
        }

        /// <summary>
        /// Parse a database value back to a typed value
        /// </summary>
        /// <param name="value">The value from the database</param>
        /// <returns>The metadata dictionary</returns>
        public override Dictionary<string, object> Parse(object value)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>((string)value);
        }
    }
}
