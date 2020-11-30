using System;
using System.Collections.Generic;
using Automatonymous;
using MassTransitSagaDeadlock.Worker.Auxiliary;
using MassTransitSagaDeadlock.Worker.Commands;
using MassTransitSagaDeadlock.Worker.Events;

namespace MassTransitSagaDeadlock.Worker.Saga
{
    public class TransferSaga : MassTransitStateMachine<TransferSagaState>
    {
        public State Initiated { get; private set; }
        public State WithdrawCompleted { get; private set; }
        public State DepositCompleted { get; private set; }
        public State WithdrawFailed { get; private set; }
        public State DepositFailed { get; private set; }
        public State Compensated { get; private set; }
        public State CompletelyFailed { get; private set; }
        public State SuccessfullyCompleted { get; private set; }

        public Event<TransferFundsCommand> TransferFundsCommand { get; private set; }

        public Event<WithdrawFundsCompletedEvent> WithdrawFundsCompleted { get; private set; }

        public Event<DepositFundsCompletedEvent> DepositFundsCompleted { get; private set; }

        public TransferSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => TransferFundsCommand, x => x.CorrelateById(context => context.Message.TransferFundsId));

            Event(() => WithdrawFundsCompleted, x => x.CorrelateById(context => context.Message.TransferFundsId));

            Event(() => DepositFundsCompleted, x => x.CorrelateById(context => context.Message.TransferFundsId));

            Initially(
                When(TransferFundsCommand)
                    .Then(x =>
                    {
                        x.Instance.CorrelationId = x.Data.TransferFundsId;
                        x.Instance.CreatedAtDatetime = x.Data.CreatedAt;
                        x.Instance.StartedAtDatetime = DateTime.UtcNow;
                        x.Instance.FromWalletId = x.Data.FromWalletId;
                        x.Instance.ToWalletId = x.Data.ToWalletId;
                        x.Instance.Amount = x.Data.Amount;
                        x.Instance.Comment = x.Data.Comment;
                        x.Instance.Metadata = x.Data.Metadata;
                        x.Instance.Errors = new List<Error>();
                    })
                    .TransitionTo(Initiated)
                    .Send(new Uri("queue:WithdrawFunds"),
                        x => new WithdrawFundsCommand
                        {
                            TransferFundsId = x.Instance.CorrelationId,
                            WalletId = x.Instance.FromWalletId,
                            Amount = x.Instance.Amount,
                            Comment = x.Instance.Comment,
                            Metadata = x.Instance.Metadata
                        })
                    .Publish(x => GetTransferSagaStateUpdatedEvent(x.Instance)));

            During(Initiated,
                When(WithdrawFundsCompleted)
                    .IfElse(x => x.Data.IsFailed, b => b
                        .Then(x =>
                        {
                            if (x.Data.Errors != null)
                                foreach (var error in x.Data.Errors)
                                {
                                    x.Instance.Errors.Add(new Error
                                    {
                                        ErrorCode = error.ErrorCode,
                                        ErrorMessage = error.ErrorMessage,
                                        FriendlyMessage = error.FriendlyMessage
                                    });
                                }
                        })
                        .TransitionTo(WithdrawFailed), b => b
                        .TransitionTo(WithdrawCompleted)
                        .Send(new Uri($"queue:DepositFunds"),
                            x => new DepositFundsCommand
                            {
                                TransferFundsId = x.Instance.CorrelationId,
                                WalletId = x.Instance.ToWalletId,
                                Amount = x.Instance.Amount,
                                Comment = x.Instance.Comment,
                                Metadata = x.Instance.Metadata
                            })
                        .Publish(x => GetTransferSagaStateUpdatedEvent(x.Instance))));

            During(WithdrawCompleted,
                When(DepositFundsCompleted)
                    .IfElse(x => x.Data.IsFailed, b => b
                        .Then(x =>
                        {
                            if (x.Data.Errors != null)
                                foreach (var error in x.Data.Errors)
                                {
                                    x.Instance.Errors.Add(new Error
                                    {
                                        ErrorCode = error.ErrorCode,
                                        ErrorMessage = error.ErrorMessage,
                                        FriendlyMessage = error.FriendlyMessage
                                    });
                                }
                        })
                        .TransitionTo(DepositFailed)
                        .Send(new Uri($"queue:DepositFunds"),
                            x => new DepositFundsCommand
                            {
                                TransferFundsId = x.Instance.CorrelationId,
                                WalletId = x.Instance.FromWalletId,
                                Amount = x.Instance.Amount,
                                Comment = x.Instance.Comment,
                                Metadata = x.Instance.Metadata
                            })
                        .TransitionTo(Compensated)
                        .Publish(x => GetTransferSagaStateUpdatedEvent(x.Instance)), b => b
                        .TransitionTo(DepositCompleted)
                        .Publish(x => GetTransferSagaStateUpdatedEvent(x.Instance))));

            During(Compensated,
                When(DepositFundsCompleted)
                    .IfElse(x => x.Data.IsFailed, b => b
                            .Then(x =>
                            {
                                if (x.Data.Errors != null)
                                    foreach (var error in x.Data.Errors)
                                    {
                                        x.Instance.Errors.Add(new Error
                                        {
                                            ErrorCode = error.ErrorCode,
                                            ErrorMessage = error.ErrorMessage,
                                            FriendlyMessage = error.FriendlyMessage
                                        });
                                    }
                            })
                            .TransitionTo(CompletelyFailed),
                        b => b.TransitionTo(SuccessfullyCompleted)));

        }

        private static TransferSagaStateUpdatedEvent GetTransferSagaStateUpdatedEvent(TransferSagaState sagaState)
        {
            return new TransferSagaStateUpdatedEvent
            {
                CorrelationId = sagaState.CorrelationId,
                FromWalletId = sagaState.FromWalletId,
                ToWalletId = sagaState.ToWalletId,
                State = sagaState.CurrentState,
                CreatedAt = DateTime.UtcNow,
                StartedAt = sagaState.StartedAtDatetime,
                Amount = sagaState.Amount,
                Comment = sagaState.Comment,
                Errors = sagaState.Errors,
            };
        }
    }
}