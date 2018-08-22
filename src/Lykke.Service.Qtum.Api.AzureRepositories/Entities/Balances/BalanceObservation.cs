using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.Balances;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances
{
    public class BalanceObservation : AzureTableEntity, IBalanceObservation
    {
        [IgnoreProperty]
        public string Address { get => RowKey; set => RowKey = value; }
    }
}
