using System;
using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions
{
    public class TransactionObservation : AzureTableEntity, ITransactionObservation
    {
        [IgnoreProperty]
        public Guid OperationId { get => new Guid(RowKey); set => RowKey = value.ToString(); }
    }
}
