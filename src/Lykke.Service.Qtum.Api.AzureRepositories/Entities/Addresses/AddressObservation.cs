using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses
{
    public class AddressObservation : AzureTableEntity, IAddressObservation
    {
        [IgnoreProperty]
        public string Address { get => RowKey; set => RowKey = value; }

        [IgnoreProperty]
        public AddressObservationType Type
        {
            get => (AddressObservationType)Enum.Parse(typeof(AddressObservationType), PartitionKey);
            set => PartitionKey = Enum.GetName(typeof(AddressObservationType), value);
        }
    }
}
