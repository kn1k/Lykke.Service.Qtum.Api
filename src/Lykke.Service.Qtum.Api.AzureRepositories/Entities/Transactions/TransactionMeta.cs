using System;
using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions
{
    public class TransactionMeta : AzureTableEntity, ITransactionMeta
    {
        [IgnoreProperty]
        public Guid OperationId { get => new Guid(RowKey); set => RowKey = value.ToString(); }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        [IgnoreProperty]
        public string AssetId { get; set; }

        public string Amount { get; set; }

        public bool IncludeFee { get; set; }

        public TransactionState State { get; set; }

        public string Error { get; set; }

        public string Hash { get; set; }
        
        public string TxId { get; set; }
        
        public string Fee { get; set; }

        public DateTime? CreateTimestamp { get; set; }

        public DateTime? SignTimestamp { get; set; }

        public DateTime? BroadcastTimestamp { get; set; }

        public DateTime? CompleteTimestamp { get; set; }

        public long BlockCount { get; set; }
        
        public TransactionType TransactionType { get; set; }       
    }
}
