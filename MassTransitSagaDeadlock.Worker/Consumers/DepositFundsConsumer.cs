using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitSagaDeadlock.Worker.Commands;
using MassTransitSagaDeadlock.Worker.Events;

namespace MassTransitSagaDeadlock.Worker.Consumers
{
    public sealed class DepositFundsConsumer : IConsumer<DepositFundsCommand>
    {
        public async Task Consume(ConsumeContext<DepositFundsCommand> context)
        {
            var response = new DepositFundsCompletedEvent
            {
                TransferFundsId = context.Message.TransferFundsId,
                WalletId = context.Message.WalletId,
                ExternalTransactionId = context.Message.ExternalTransactionId,
                Amount = context.Message.Amount,
                CompletedAt = DateTime.UtcNow,
                Comment = context.Message.Comment
            };
            await context.Publish(response);
        }
    }
}