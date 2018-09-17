using System;
using Lykke.Service.Qtum.Api.Core.Services;

namespace Lykke.Service.Qtum.Api.Core.Domain.Transactions
{
    public interface ITransactionMeta
    {
        Guid OperationId { get; set; }

        string FromAddress { get; set; }

        string ToAddress { get; set; }

        string AssetId { get; set; }

        string Amount { get; set; }

        bool IncludeFee { get; set; }

        TransactionState State { get; set; }

        string Error { get; set; }

        string Hash { get; set; }
        
        string TxId { get; set; }
        
        string Fee { get; set; }

        Int64 BlockCount { get; set; }

        TransactionType TransactionType { get; set; }

        DateTime? CreateTimestamp { get; set; }

        DateTime? SignTimestamp { get; set; }

        DateTime? BroadcastTimestamp { get; set; }

        DateTime? CompleteTimestamp { get; set; }
    }
}
