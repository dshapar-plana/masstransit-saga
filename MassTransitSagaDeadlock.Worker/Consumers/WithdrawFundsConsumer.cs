using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitSagaDeadlock.Worker.Commands;
using MassTransitSagaDeadlock.Worker.Events;

namespace MassTransitSagaDeadlock.Worker.Consumers
{
    public sealed class WithdrawFundsConsumer : IConsumer<WithdrawFundsCommand>
    {
        public async Task Consume(ConsumeContext<WithdrawFundsCommand> context)
        {
            var response = new WithdrawFundsCompletedEvent
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