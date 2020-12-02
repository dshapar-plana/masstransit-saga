using System;
using System.Collections.Generic;

namespace MassTransitSagaDeadlock.Console.Client
{
    public class TransferFundsCommand
    {
        public Guid TransferFundsId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}