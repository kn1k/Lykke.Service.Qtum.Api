using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using Lykke.Service.Qtum.Api.Core.Services;
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
        private readonly ITransactionOutputsService _transactionOutputsService;
        
        private const int RetryCount = 4;

        private const int RetryTimeout = 1;

        private readonly Policy _policy;

        public BlockchainService(Network network, IInsightApiService insightApiService, ITransactionOutputsService transactionOutputsService)
        {
            _network = network;
            _insightApiService = insightApiService;
            _transactionOutputsService = transactionOutputsService;

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
                var result = await _insightApiService.GetUtxoAsync(address);
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
                    return await _insightApiService.GetAddrTxsAsync(address, from, to);
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

        /// <inheritdoc/>
        public async Task<IList<Coin>> GetUnspentOutputsAsync(string address, int minConfirmationCount)
        {
            var utxos = await _policy.ExecuteAsync(async () =>
            {
                return await _insightApiService.GetUtxoAsync(ParseAddress(address));
            });

            return utxos
                    .Where(p => p.Confirmations >= minConfirmationCount)
                    .Select(source =>
                    {
                        return new Coin(
                            new OutPoint(uint256.Parse(source.Txid), source.Vout),
                            new TxOut(new Money(ulong.Parse(source.Satoshis)),
                            source.ScriptPubKey.ToScript()));
                    }).ToList();
        }

        /// <inheritdoc/>
        public async Task<string> CreateUnsignSendTransactionAsync(string fromAddress, string toAddress, long amount, bool includeFee)
        {
            var builder = new TransactionBuilder();

            var coins = (await _transactionOutputsService.GetUnspentOutputs(fromAddress)).ToList();
            var balance = coins.Select(o => o.Amount).Sum(o => o.Satoshi);


            if (balance > amount &&
                balance - amount < new TxOut(Money.Zero, ParseAddress(fromAddress)).GetDustThreshold(builder.StandardTransactionPolicy.MinRelayTxFee).Satoshi)
            {
                amount = balance;
            }

            return await SendWithChange(builder, coins, ParseAddress(toAddress), new Money(balance), new Money(amount), ParseAddress(fromAddress), includeFee);
        }

        private async Task<string> SendWithChange(TransactionBuilder builder, List<Coin> coins, IDestination destination, Money balance, Money amount, IDestination changeDestination, bool includeFee)
        {
            if (amount.Satoshi <= 0)
                throw new Exception("Amount can't be less or equal to zero");

            builder.AddCoins(coins)
                   .Send(destination, amount)
                   .SetChange(changeDestination);

            /*
            var calculatedFee = await _feeService.CalcFeeForTransaction(builder);
            var requiredBalance = amount + (includeFee ? Money.Zero : calculatedFee);

            if (balance < requiredBalance)
                throw new Exception($"The sum of total applicable outputs is less than the required : {requiredBalance} satoshis.");

            if (includeFee)
            {
                if (calculatedFee > amount)
                    throw new Exception($"The sum of total applicable outputs is less than the required fee:{calculatedFee} satoshis.");
                builder.SubtractFees();
                amount = amount - calculatedFee;
            }

            builder.SendFees(calculatedFee);*/

            var tx = builder.BuildTransaction(false);
            var usedCoins = tx.Inputs.Select(input => coins.First(o => o.Outpoint == input.PrevOut)).ToList(); // do we need them?

            return tx.ToHex();
        }
    }
}
