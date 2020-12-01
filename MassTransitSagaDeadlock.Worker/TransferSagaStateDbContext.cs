using System.Collections.Generic;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransitSagaDeadlock.Worker.Saga;
using Microsoft.EntityFrameworkCore;

namespace MassTransitSagaDeadlock.Worker
{
    public class TransferSagaStateDbContext :
        SagaDbContext
    {
        public TransferSagaStateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new TransferSagaStateMap(); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransferSagaState>().HasKey(a => a.CorrelationId);
            modelBuilder.Entity<TransferSagaState>().Ignore(a => a.Metadata);
            modelBuilder.Entity<TransferSagaState>().Ignore(a => a.Errors);
            base.OnModelCreating(modelBuilder);
        }
    }
}