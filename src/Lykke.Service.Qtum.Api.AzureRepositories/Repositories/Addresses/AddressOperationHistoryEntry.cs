using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses;
using Lykke.Service.Qtum.Api.AzureRepositories.Repositories;
using Lykke.Service.Qtum.Api.Core.Repositories.Addresses;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Addresses
{
    public class AddressOperationHistoryEntryRepository : AzureRepository<AddressOperationHistoryEntry>, IAddressOperationHistoryEntryRepository<AddressOperationHistoryEntry>
    {
        public AddressOperationHistoryEntryRepository(IReloadingManager<string> connectionStringManager, ILogFactory log) : base(connectionStringManager, log)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(AddressOperationHistoryEntry);
        }

        public async Task<IEnumerable<AddressOperationHistoryEntry>> GetByAddressAsync(int take, string partitionKey, string address)
        {
            var page = new PagingInfo { ElementCount = take };
            var query = new TableQuery<AddressOperationHistoryEntry>()
                 .Where(TableQuery.CombineFilters(
                     TableQuery.GenerateFilterCondition(nameof(AddressOperationHistoryEntry.PartitionKey), QueryComparisons.Equal, partitionKey),
                     TableOperators.And,
                     TableQuery.GenerateFilterCondition(nameof(AddressOperationHistoryEntry.Address), QueryComparisons.Equal, address)));
            return await _tableStorage.ExecuteQueryWithPaginationAsync(query, page);
        }
    }
}
