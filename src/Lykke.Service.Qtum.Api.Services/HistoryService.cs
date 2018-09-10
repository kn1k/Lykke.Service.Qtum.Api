using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Lykke.Service.Qtum.Api.Core.Repositories.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Services
{
    public class HistoryService<TAddressHistory, TAddressObservation, TAddressOperation> : IHistoryService<TAddressHistory, TAddressObservation, TAddressOperation>
        where TAddressHistory : IAddressHistoryEntry
        where TAddressObservation : IAddressObservation
        where TAddressOperation : IAddressOperationHistoryEntry
    {
        private readonly IAddressHistoryEntryRepository<TAddressHistory> _addressHistoryEntryRepository;
        private readonly IAddressObservationRepository<TAddressObservation> _addressObservationRepository;
        private readonly IAddressOperationHistoryEntryRepository<TAddressOperation> _addressOperationHistoryEntryRepository;

        public HistoryService(IAddressHistoryEntryRepository<TAddressHistory> addressHistoryEntryRepository, IAddressObservationRepository<TAddressObservation> addressObservationRepository, IAddressOperationHistoryEntryRepository<TAddressOperation> addressOperationHistoryEntryRepository)
        {
            _addressHistoryEntryRepository = addressHistoryEntryRepository;
            _addressObservationRepository = addressObservationRepository;
            _addressOperationHistoryEntryRepository = addressOperationHistoryEntryRepository;
        }

        public async Task<bool> AddAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        public Task<bool> AddAddressOperationHistoryAsync(TAddressOperation operationHistoryEntry)
        {
            throw new NotImplementedException();
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

        public Task<(string continuation, IEnumerable<TAddressObservation> items)> GetAddressObservationAsync(int pageSize, string continuation = null, string partitionKey = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TAddressOperation>> GetAddressOperationHistoryAsync(int take, string partitionKey, string address)
        {
            return await _addressOperationHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address);
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
    }
}
