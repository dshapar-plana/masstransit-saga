using System;
using System.Data;
using Dapper;

namespace MassTransitSagaDeadlock.Worker.Auxiliary
{
    public sealed class MsSqlGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            if (value == null || string.IsNullOrEmpty(value as string))
                return Guid.Empty;

            return new Guid((string)value);
        }
    }
}
