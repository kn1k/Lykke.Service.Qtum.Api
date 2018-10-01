using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface IHistoryService<TAddressHistory, TAddressObservation>
    {
        /// <summary>
        /// Check is address history already observed
        /// </summary>
        /// <param name="addressObservation">Address observation entity</param>
        /// <returns>true if already observed</returns>
        Task<bool> IsAddressObservedAsync(TAddressObservation addressObservation);

        /// <summary>
        /// Observe address history
        /// </summary>
        /// <param name="addressObservation">Address observation entity</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> AddAddressObservationAsync(TAddressObservation addressObservation);

        /// <summary>
        /// Stop observe address history
        /// </summary>
        /// <param name="addressObservation"></param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task<bool> RemoveAddressObservationAsync(TAddressObservation addressObservation);

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="pageSize">Amount of address observation</param>
        /// <param name="continuation">continuation data</param>
        /// <param name="partitionKey">partition key for azure storage</param>
        /// <returns>continuation data and observerd addresses</returns>
        Task<(string continuation, IEnumerable<TAddressObservation> items)> GetAddressObservationAsync(int pageSize, string continuation = null, string partitionKey = null);

        /// <summary>
        /// Get stored address history after specific hash
        /// </summary>
        /// <param name="take">Amount of history entries</param>
        /// <param name="partitionKey">partition key for azure storage</param>
        /// <param name="address">Address</param>
        /// <param name="afterHash">Block hash</param>
        /// <returns>Address history</returns>
        Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash = null, string continuation = null);

        /// <summary>
        /// Get stored address history
        /// </summary>
        /// <param name="take">Amount of history entries</param>
        /// <param name="continuation">continuation data</param>
        /// <param name="partitionKey">partition key for azure storage</param>
        /// <returns>Address history</returns>
        Task<(string continuation, IEnumerable<TAddressHistory> items)> GetHistoryAsync(int take, string continuation, string partitionKey = null);

        /// <summary>
        /// Save address history entry
        /// </summary>
        /// <param name="addressHistoryEntry">Address history entry</param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task<bool> InsertAddressHistoryAsync(TAddressHistory addressHistoryEntry);

        /// <summary>
        /// Get pending blocks from history
        /// </summary>
        /// <param name="take">Amount of history entries</param>
        /// <param name="continuation">continuation data</param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressPendingHistoryAsync(int take, string continuation = null);

        /// <summary>
        /// Remove history entry
        /// </summary>
        /// <param name="addressHistoryEntry">Address history entry</param>
        /// <returns>true if removed, false if not exist</returns>
        Task<bool> RemoveAddressHistoryEntryAsync(TAddressHistory addressHistoryEntry);

        /// <summary>
        /// Update observed addresses history
        /// </summary>
        ///  <param name="type">Address observation type</param>
        /// <param name="pageSize">Update page size</param>
        /// <returns></returns>
        Task UpdateObservedAddressHistoryAsync(AddressObservationType type, int pageSize = 10);
    }
}
