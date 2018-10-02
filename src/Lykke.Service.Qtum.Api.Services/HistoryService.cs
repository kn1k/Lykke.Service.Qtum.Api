using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses;
using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using Lykke.Service.Qtum.Api.Core.Helpers;
using Lykke.Service.Qtum.Api.Core.Repositories.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;

namespace Lykke.Service.Qtum.Api.Services
{
    public class HistoryService<TAddressHistory, TAddressObservation> : IHistoryService<TAddressHistory, TAddressObservation>
        where TAddressHistory : IAddressHistoryEntry
        where TAddressObservation : IAddressObservation
    {
        private readonly IAddressHistoryEntryRepository<TAddressHistory> _addressHistoryEntryRepository;
        private readonly IAddressObservationRepository<TAddressObservation> _addressObservationRepository;
        private readonly IBlockchainService _blockchainService;
        private readonly IAssetService _assetService;
        private readonly ILog _log;

        public HistoryService(ILogFactory logFactory, IAddressHistoryEntryRepository<TAddressHistory> addressHistoryEntryRepository, IAddressObservationRepository<TAddressObservation> addressObservationRepository, IBlockchainService blockchainService, IAssetService assetService)
        {
            _addressHistoryEntryRepository = addressHistoryEntryRepository;
            _addressObservationRepository = addressObservationRepository;
            _blockchainService = blockchainService;
            _assetService = assetService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<bool> AddAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        ///</inheritdoc>
        public async Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash = null, string continuation = null)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            if (partitionKey == null) throw new ArgumentNullException(nameof(partitionKey));

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

        ///</inheritdoc>
        public async Task<(string continuation, IEnumerable<TAddressObservation> items)> GetAddressObservationAsync(int pageSize, string continuation = null, string partitionKey = null)
        {
            return await _addressObservationRepository.GetAsync(pageSize, continuation, partitionKey);
        }

        ///</inheritdoc>
        public Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressPendingHistoryAsync(int take, string continuation = null)
        {
            throw new NotImplementedException();
        }

        ///</inheritdoc>
        public Task<(string continuation, IEnumerable<TAddressHistory> items)> GetHistoryAsync(int take, string continuation, string partitionKey = null)
        {
            throw new NotImplementedException();
        }

        ///</inheritdoc>
        public async Task<bool> InsertAddressHistoryAsync(TAddressHistory addressHistoryEntry)
        {
            return await _addressHistoryEntryRepository.CreateIfNotExistsAsync(addressHistoryEntry);
        }

        ///</inheritdoc>
        public async Task<bool> IsAddressObservedAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.IsExistAsync(addressObservation);
        }

        ///</inheritdoc>
        public async Task<bool> RemoveAddressHistoryEntryAsync(TAddressHistory addressHistoryEntry)
        {
            return await _addressHistoryEntryRepository.DeleteIfExistAsync(addressHistoryEntry);
        }

        ///</inheritdoc>
        public async Task<bool> RemoveAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.DeleteIfExistAsync(addressObservation);
        }

        ///</inheritdoc>
        public async Task UpdateObservedAddressHistoryAsync(AddressObservationType type, int pageSize = 10)
        {
            (string continuation, IEnumerable<TAddressObservation> items) addressObservation;
            string continuation = null;

            do
            {
                addressObservation = await GetAddressObservationAsync(pageSize, continuation, Enum.GetName(typeof(AddressObservationType), type));

                if (addressObservation.items.Any())
                {
                    continuation = addressObservation.continuation;

                    foreach (var observedAddress in addressObservation.items)
                    {
                        try
                        {
                            var transactionInfoItems = await _blockchainService.GetAddressTransactionsInfoAsync(_blockchainService.ParseAddress(observedAddress.Address));

                            if (transactionInfoItems != null)
                            {
                                foreach (var transactionInfo in transactionInfoItems)
                                {
                                    var addressHistoryEntry = GetAddressHistoryEntry(transactionInfo, observedAddress);

                                    if (addressHistoryEntry == null) continue;

                                    if (!await InsertAddressHistoryAsync((TAddressHistory)addressHistoryEntry))
                                    {
                                        _log.Warning("Unable to insert address history entry into the store");
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _log.Error(e, $"Exception while getting transactions history for address {observedAddress.Address}");
                        }
                    }
                }

            } while (continuation != null);
        }

        private IAddressHistoryEntry GetAddressHistoryEntry(ITxInfo tx, TAddressObservation addrObservation)
        {
            // need to process 1 -> n, n -> 1
            // but do it like in bitcoin* APIs

            var requestedAddress = addrObservation.Address;
            var nfi = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = "." };

            var isSending = tx.Vin.Where(p => p.Addr == requestedAddress).Sum(p => p.Value) >=
                            tx.Vout.Where(p => p.ScriptPubKey.Addresses[0] == requestedAddress).Sum(p => decimal.Parse(p.Value, nfi));

            if (isSending == (addrObservation.Type == AddressObservationType.From))
            {

                string from;
                string to;
                decimal amount;


                if (isSending)
                {
                    from = requestedAddress;
                    to = tx.Vout.Select(o => o.ScriptPubKey.Addresses[0]).FirstOrDefault(o => o != null && o != requestedAddress) ?? requestedAddress;
                    amount = tx.Vout.Where(o => o.ScriptPubKey.Addresses[0] != requestedAddress).Sum(o => decimal.Parse(o.Value, nfi));
                }
                else
                {
                    to = requestedAddress;
                    from = tx.Vin.Select(o => o.Addr).FirstOrDefault(o => o != null && o != requestedAddress) ?? requestedAddress;
                    amount = tx.Vout.Where(o => o.ScriptPubKey.Addresses[0] == requestedAddress).Sum(o => decimal.Parse(o.Value, nfi));
                }

                return new AddressHistoryEntry
                {
                    FromAddress = from,
                    ToAddress = to,
                    Amount = (amount * (decimal)Math.Pow(10, _assetService.GetQtumAsset().Accuracy)).ToString("0"),
                    AssetId = _assetService.GetQtumAsset().Id,
                    Type = addrObservation.Type,
                    TransactionType = isSending ? TransactionType.send : TransactionType.receive,
                    TransactionTimestamp = UnixTimeHelper.UnixTimeStampToDateTime(tx.Time),
                    Hash = tx.Txid,
                    BlockCount = tx.Blockheight
                };
            }
            else
            {
                return null;
            }
        }
    }
}
