using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    /// <summary>
    /// Errors collection handler
    /// </summary>
    public sealed class MsSqlErrorsCollectionTypeHandler : SqlMapper.TypeHandler<List<Error>>
    {
        /// <summary>
        /// Set serialized value
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="errors"></param>
        public override void SetValue(IDbDataParameter parameter, List<Error> errors)
        {
            parameter.Value = JsonConvert.SerializeObject(errors);
        }

        /// <summary>
        /// Parse serialized errors
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The errors</returns>
        public override List<Error> Parse(object value)
        {
            return JsonConvert.DeserializeObject<List<Error>>((string)value);
        }
    }
}
