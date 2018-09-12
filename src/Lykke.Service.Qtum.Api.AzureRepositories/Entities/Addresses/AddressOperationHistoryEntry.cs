using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses
{
    public class AddressOperationHistoryEntry : AzureTableEntity, IAddressOperationHistoryEntry
    {
        public Guid OperationId { get; set; }

        [IgnoreProperty]
        public string Hash { get => RowKey; set => RowKey = value; }

        public string Address { get; set; }

        [IgnoreProperty]
        public AddressObservationType Type
        {
            get => (AddressObservationType)Enum.Parse(typeof(AddressObservationType), PartitionKey);
            set => PartitionKey = Enum.GetName(typeof(AddressObservationType), value);
        }

        public DateTime TransactionTimestamp { get; set; }
    }
}
