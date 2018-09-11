using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses
{
    public class AddressHistoryEntry : AzureTableEntity, IAddressHistoryEntry
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public DateTime? TransactionTimestamp { get; set; }

        [IgnoreProperty] public string AssetId { get; set; }

        public string Amount { get; set; }

        [IgnoreProperty]
        public string Hash
        {
            get => RowKey;
            set => RowKey = value;
        }

        public long BlockCount { get; set; }

        public TransactionType TransactionType { get; set; }

        [IgnoreProperty]
        public AddressObservationType Type
        {
            get => (AddressObservationType) Enum.Parse(typeof(AddressObservationType), PartitionKey);
            set => PartitionKey = Enum.GetName(typeof(AddressObservationType), value);
        }
    }
}
