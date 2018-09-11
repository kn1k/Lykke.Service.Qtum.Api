using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Lykke.Service.Qtum.Api.Core.Repositories.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Services
{
    public class HistoryService<TAddressHistory, TAddressObservation> : IHistoryService<TAddressHistory, TAddressObservation>
        where TAddressHistory : IAddressHistoryEntry
        where TAddressObservation : IAddressObservation
    {
        private readonly IAddressHistoryEntryRepository<TAddressHistory> _addressHistoryEntryRepository;
        private readonly IAddressObservationRepository<TAddressObservation> _addressObservationRepository;
        private readonly ILog _log;

        public HistoryService(ILogFactory logFactory, IAddressHistoryEntryRepository<TAddressHistory> addressHistoryEntryRepository, IAddressObservationRepository<TAddressObservation> addressObservationRepository)
        {
            _addressHistoryEntryRepository = addressHistoryEntryRepository;
            _addressObservationRepository = addressObservationRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task<bool> AddAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        public async Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash = null, string continuation = null)
        {
            if (address != null && partitionKey != null)
            {
                if (afterHash != null)
                {
                    var afterRecord = await _addressHistoryEntryRepository.GetAsync(afterHash, partitionKey);
                    var afterBlockCount = afterRecord.BlockCount;

                    return await _addressHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address, afterBlockCount, continuation);
                }
                else
                {
                    return await _addressHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address, continuation: continuation);
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public async Task<(string continuation, IEnumerable<TAddressObservation> items)> GetAddressObservationAsync(int pageSize, string continuation = null, string partitionKey = null)
        {
            return await _addressObservationRepository.GetAsync(pageSize, continuation, partitionKey);
        }

        public Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressPendingHistoryAsync(int take, string continuation = null)
        {
            throw new NotImplementedException();
        }

        public Task<(string continuation, IEnumerable<TAddressHistory> items)> GetHistoryAsync(int take, string continuation, string partitionKey = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAddressHistoryAsync(TAddressHistory addressHistoryEntry)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsAddressObservedAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.IsExistAsync(addressObservation);
        }

        public Task<bool> RemoveAddressHistoryEntryAsync(TAddressHistory addressHistoryEntry)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.DeleteIfExistAsync(addressObservation);
        }

        public async Task UpdateObservedAddressHistoryAsync(int pageSize = 10)
        {
            (string continuation, IEnumerable<TAddressObservation> items) addressObservation;
            string continuation = null;

            do
            {
                addressObservation = await GetAddressObservationAsync(pageSize, continuation);

                if (addressObservation.items.Any())
                {
                    continuation = addressObservation.continuation;

                    foreach (var observedAddress in addressObservation.items)
                    {
                        try
                        {
                            // blockchainService - InsightAPI addr / txs
                        }
                        catch (Exception e)
                        {
                            _log.Error(e, $"Exception while getting transactions history for address {observedAddress.Address}");
                        }
                    }
                }

            } while (continuation != null);
        }
    }
}
