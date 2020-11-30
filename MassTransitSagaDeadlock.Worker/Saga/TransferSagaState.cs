using System;
using System.Collections.Generic;
using Automatonymous;
using Dapper.Contrib.Extensions;
using MassTransitSagaDeadlock.Worker.Auxiliary;

namespace MassTransitSagaDeadlock.Worker.Saga
{
    public class TransferSagaState : SagaStateMachineInstance
    {
        [ExplicitKey]
        public Guid CorrelationId { get; set; }
        public DateTime CreatedAtDatetime { get; set; }
        public DateTime StartedAtDatetime { get; set; }
        public string CurrentState { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public List<Error> Errors { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        
        //DO NOT REMOVE
        //public byte[] RowVersion { get; set; }
    }
}