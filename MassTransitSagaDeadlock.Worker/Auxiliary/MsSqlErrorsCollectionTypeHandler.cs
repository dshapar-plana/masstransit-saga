using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    public sealed class MsSqlErrorsCollectionTypeHandler : SqlMapper.TypeHandler<List<Error>>
    {
        public override void SetValue(IDbDataParameter parameter, List<Error> errors)
        {
            parameter.Value = JsonConvert.SerializeObject(errors);
        }

        public override List<Error> Parse(object value)
        {
            return JsonConvert.DeserializeObject<List<Error>>((string)value);
        }
    }
}
