using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using Lykke.Service.Qtum.Api.Core.Services;
using NBitcoin;

using Polly;

namespace Lykke.Service.Qtum.Api.Services
{
    /// <summary>
    /// Blockchain service
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        private readonly Network _network;
        private readonly IInsightApiService _insightApiService;
        
        private const int RetryCount = 4;

        private const int RetryTimeout = 1;

        private readonly Policy _policy;

        public BlockchainService(Network network, IInsightApiService insightApiService)
        {
            _network = network;
            _insightApiService = insightApiService;
            
            _policy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(RetryTimeout));
        }

        /// <inheritdoc/>
        public Network GetNetwork()
        {
            return _network;
        }

        /// <inheritdoc/>
        public BitcoinAddress ParseAddress(string address)
        {
            return BitcoinAddressCreate(address);
        }

        /// <inheritdoc/>
        public bool IsAddressValid(string address, out Exception exception)
        {
            try
            {
                exception = null;
                return BitcoinAddressCreate(address) != null;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<long> GetBlockCountAsync()
        {
            var policyResult = _policy.ExecuteAsync(async () =>
            {
                var result = await _insightApiService.GetStatus();
                return result.Info.Blocks;
            });

            return await policyResult;
        }

        /// <inheritdoc/>
        public async Task<BigInteger> GetAddressBalanceAsync(BitcoinAddress address)
        {
            var policyResult = _policy.ExecuteAsync(async () =>
            {
                var result = await _insightApiService.GetUtxo(address);
                if (result.Any())
                {
                    return result.Select(x => BigInteger.Parse(x.Satoshis)).Aggregate((currentSum, item)=> currentSum + item);
                }
                else
                {
                    return 0;
                }

            });

            return await policyResult;
        }

        /// <inheritdoc/>
        public bool IsAddressValid(string address)
        {
            try
            {
                return BitcoinAddressCreate(address) != null;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private BitcoinAddress BitcoinAddressCreate(string address)
        {
            return BitcoinAddress.Create(address, _network);
        }

        /// <inheritdoc/>
        public async Task<List<IItem>> GetAddressTransactionsInfoAsync(BitcoinAddress address)
        {
            List<IItem> result = null;
            const int pageSize = 50;
            int from = 0, to = from + pageSize;

            var needAdditionalRequest = false;

            do
            {
                var policyResult = await _policy.ExecuteAsync(async () =>
                {
                    return await _insightApiService.GetAddrTxs(address, from, to);
                });

                if (policyResult.TotalItems > 0)
                {
                    if (result == null)
                    {
                        result = new List<IItem>();
                    }

                    result.AddRange(policyResult.Items);

                    if (policyResult.TotalItems > to)
                    {
                        from = to;
                        to = Math.Min(to + pageSize, policyResult.TotalItems);
                        needAdditionalRequest = true;
                    }
                    else
                    {
                        needAdditionalRequest = false;
                    }
                }

            } while (needAdditionalRequest);
            
            return result;
        }
    }
}
