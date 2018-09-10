using System;
using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Balances;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances
{
    public class AddressBalance : AzureTableEntity, IAddressBalance
    {
        [IgnoreProperty]
        public string Address { get => RowKey; set => RowKey = value; }

        [IgnoreProperty]
        public string AssetId { get; set; }

        public string Balance { get; set; }
        
        public Int64 Block { get; set; }
    }
}
