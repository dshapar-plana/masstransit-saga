using System;
using System.Collections.Generic;

namespace MassTransitSagaDeadlock.Worker.Commands
{
    public class DepositFundsCommand
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
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the external transaction identifier.
        /// </summary>
        /// <value>
        /// The external transaction identifier.
        /// </value>
        public string ExternalTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public Dictionary<string, object> Metadata { get; set; }
    }
}