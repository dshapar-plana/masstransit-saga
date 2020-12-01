using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransitSagaDeadlock.Worker.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransitSagaDeadlock.Worker
{
    public class TransferSagaStateMap :
        SagaClassMap<TransferSagaState>
    {
        protected override void Configure(EntityTypeBuilder<TransferSagaState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);

            // If using Optimistic concurrency, otherwise remove this property
            //entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}