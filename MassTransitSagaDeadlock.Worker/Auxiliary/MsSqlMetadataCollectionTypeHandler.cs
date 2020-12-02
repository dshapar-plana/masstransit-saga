using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    public sealed class MsSqlMetadataCollectionTypeHandler : SqlMapper.TypeHandler<Dictionary<string, object>>
    {
        public override void SetValue(IDbDataParameter parameter, Dictionary<string, object> metadata)
        {
            parameter.Value = JsonConvert.SerializeObject(metadata);
        }

        public override Dictionary<string, object> Parse(object value)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>((string)value);
        }
    }
}
