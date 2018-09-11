using System;
using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions
{
    public class TransactionBody : AzureTableEntity, ITransactionBody
    {
        [IgnoreProperty]
        public Guid OperationId { get => new Guid(RowKey); set => RowKey = value.ToString(); }
        public string UnsignedTransaction { get; set; }
        public string SignedTransaction { get; set; }
    }
}
