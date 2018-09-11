using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Addresses
{
    public interface IAddressHistoryEntryRepository<THistoryEntry> : IRepository<THistoryEntry>
        where THistoryEntry : IAddressHistoryEntry
    {
        /// <summary>
        /// Return history entries for address after specific hash
        /// </summary>
        /// <param name="take">Amount of the returned history entry</param>
        /// <param name="partitionKey">PartitionKey for azure table storage</param>
        /// <param name="address">Address</param>
        /// <param name="afterBlockCount">Block hash</param>
        /// <param name="continuation">continuation data</param>
        /// <returns>History entries for address after specific hash</returns>
        Task<(string continuation, IEnumerable<THistoryEntry> items)> GetByAddressAsync(int take, string partitionKey, string address, long afterBlockCount = 0, string continuation = null);

        /// <summary>
        /// Return history entries by block count
        /// </summary>
        /// <param name="take">Amount of the returned history entry</param>
        /// <param name="partitionKey">PartitionKey for azure table storage</param>
        /// <param name="blockNum">Block count</param>
        /// <param name="continuation">continuation data</param>
        /// <returns>History entries by block count</returns>
        Task<(string continuation, IEnumerable<THistoryEntry> items)> GetByBlockAsync(int take, string partitionKey, long blockNum, string continuation);
    }
}
