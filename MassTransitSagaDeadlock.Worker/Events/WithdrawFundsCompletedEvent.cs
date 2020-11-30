using System;
using MassTransitSagaDeadlock.Worker.Auxiliary;

namespace MassTransitSagaDeadlock.Worker.Events
{
    public class WithdrawFundsCompletedEvent
    {
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>
        /// The transaction identifier.
        /// </value>
        public Guid TransferFundsId { get; set; }

        /// <summary>
        /// Gets or sets the wallet identifier.
        /// </summary>
        /// <value>
        /// The wallet identifier.
        /// </value>
        public int WalletId { get; set; }

        /// <summary>
        /// Gets or sets the completed at.
        /// </summary>
        /// <value>
        /// The completed at.
        /// </value>
        public DateTime CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is failed; otherwise, <c>false</c>.
        /// </value>
        public bool IsFailed { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Error[] Errors { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the external transaction identifier.
        /// </summary>
        /// <value>
        /// The external transaction identifier.
        /// </value>
        public string ExternalTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }
    }
}