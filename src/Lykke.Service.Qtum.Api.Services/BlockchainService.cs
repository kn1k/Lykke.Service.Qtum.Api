using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Services.InsightApi;
using Lykke.Service.Qtum.Api.Services.Helpers;
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
        private readonly int _confirmationsCount;

        private const int RetryCount = 4;

        private const int RetryTimeout = 1;

        private readonly Policy _policy;

        public BlockchainService(Network network, IInsightApiService insightApiService, IFeeService feeService, int confirmationsCount)
        {
            _network = network;
            _insightApiService = insightApiService;
            _confirmationsCount = confirmationsCount;

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
                var result = await _insightApiService.GetStatusAsync();
                return result.Info.Blocks;
            });

            return await policyResult;
        }

        /// <inheritdoc/>
        public async Task<BigInteger> GetAddressBalanceAsync(BitcoinAddress address)
        {
            var policyResult = _policy.ExecuteAsync(async () =>
            {
                var utxos = await _insightApiService.GetUtxoAsync(address);
                if (utxos.Any())
                {
                    return utxos
                        .Where(p => p.Confirmations >= _confirmationsCount)
                        .Select(x => BigInteger.Parse(x.Satoshis)).Aggregate(BigInteger.Zero, (currentSum, item)=> currentSum + item);
                }
                else
                {
                    return 0;
                }

            });

            return await policyResult;
        }

        /// <inheritdoc/>
        public async Task<(string txId, string error)> BroadcastSignedTransactionAsync(string signedTransaction)
        {
            var policyResult = _policy.ExecuteAsync(async () =>
            {
                var result = await _insightApiService.TxSendAsync(new RawTx { rawtx = signedTransaction}); 
                return (result.txId?.txid, result.error?.error);                          
            });

            return await policyResult;
        }

        /// <inheritdoc/>
        public async Task<ITxInfo> GetTransactionInfoByIdAsync(string id)
        {
            var policyResult = _policy.ExecuteAsync(async () =>
            {
                var result = await _insightApiService.GetTxByIdAsync(new TxId { txid = id}); 
                return result;                          
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
        public async Task<List<ITxInfo>> GetAddressTransactionsInfoAsync(BitcoinAddress address)
        {
            List<ITxInfo> result = null;
            const int pageSize = 50;
            int from = 0, to = from + pageSize;

            var needAdditionalRequest = false;

            do
            {
                var policyResult = await _policy.ExecuteAsync(async () =>
                {
                    return await _insightApiService.GetAddrTxsAsync(address, from, to);
                });

                if (policyResult.TotalItems > 0)
                {
                    if (result == null)
                    {
                        result = new List<ITxInfo>();
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

        /// <inheritdoc/>
        public async Task<IList<(long, Coin)>> GetUnspentOutputsAsync(string address)
        {
            var utxos = await _policy.ExecuteAsync(async () =>
            {
                return await _insightApiService.GetUtxoAsync(ParseAddress(address));
            });

            var confirmationsCount = _confirmationsCount <= 0 ? 1 : _confirmationsCount;
            return utxos
                    .Where(p => p.Confirmations >= confirmationsCount)
                    .Select(source =>
                    {
                        return (source.Confirmations, new Coin(
                            new OutPoint(uint256.Parse(source.Txid), source.Vout),
                            new TxOut(new Money(ulong.Parse(source.Satoshis)),
                            source.ScriptPubKey.ToScript())));
                    }).ToList();
        }        
    }
}
