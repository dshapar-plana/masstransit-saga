using System;
using System.Collections.Generic;
using MassTransitSagaDeadlock.Worker.Auxiliary;

namespace MassTransitSagaDeadlock.Worker.Events
{
    public class TransferSagaStateUpdatedEvent
    {
        public Guid TransferFundsId { get; set; }
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>
        /// The transaction identifier.
        /// </value>

        //TODO eugene make it type of enum
        public string State { get; set; }
        public Guid CorrelationId { get; set; }
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public List<Error> Errors { get; set; }
    }
}