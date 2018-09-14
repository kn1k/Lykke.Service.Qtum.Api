using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Lykke.Service.Qtum.Api.Core.Repositories.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json.Linq;

namespace Lykke.Service.Qtum.Api.Services
{
    public class TransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation, TOutput> : ITransactionService<
        TTransactionBody, TTransactionMeta, TTransactionObservation, TOutput>
        where TTransactionBody : ITransactionBody, new()
        where TTransactionMeta : ITransactionMeta, new()
        where TTransactionObservation : ITransactionObservation, new()
        where TOutput: IOutput, new()
    {
        private readonly ILog _log;
        private readonly ITransactionBodyRepository<TTransactionBody> _transactionBodyRepository;
        private readonly ITransactionMetaRepository<TTransactionMeta> _transactionMetaRepository;
        private readonly ITransactionObservationRepository<TTransactionObservation> _transactionObservationRepository;
        private readonly ISpentOutputRepository<TOutput> _spentOutputRepository;
        private readonly IBlockchainService _blockchainService;

        public TransactionService(ILogFactory logFactory, ITransactionBodyRepository<TTransactionBody> transactionBodyRepository, ITransactionMetaRepository<TTransactionMeta> transactionMetaRepository, ITransactionObservationRepository<TTransactionObservation> transactionObservationRepository, IBlockchainService blockchainService, ISpentOutputRepository<TOutput> spentOutputRepository)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
            _spentOutputRepository = spentOutputRepository;
            _blockchainService = blockchainService;
            _log = logFactory.CreateLog(this);
        }

        /// </inheritdoc>
        public async Task<string> GetUnsignSendTransactionAsync(Guid operationId, string fromAddress, string toAddress, string amount, string assetId, bool includeFee)
        {
            var transactionBody = await GetTransactionBodyByIdAsync(operationId);

            if (transactionBody == null)
            {
                _log.Info(nameof(GetUnsignSendTransactionAsync), $"Create new unsigned transaction, with id: {operationId}", 
                    JObject.FromObject(new
                    {
                        operationId,
                        fromAddress,
                        toAddress,
                        amount,
                        assetId,
                        includeFee
                    }));

                var transactionMeta = new TTransactionMeta
                {
                    OperationId = operationId,
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    AssetId = assetId,
                    Amount = amount,
                    IncludeFee = includeFee,
                    State = TransactionState.NotSigned,
                    CreateTimestamp = DateTime.Now,
                    TransactionType = TransactionType.send,
                };

                await SaveTransactionMetaAsync(transactionMeta);

                _log.Info(nameof(GetUnsignSendTransactionAsync), $"Create new txMeta, with id: {operationId}", JObject.FromObject(transactionMeta));

                var unsignedTransaction =
                    await CreateUnsignSendTransactionAsync(transactionMeta.FromAddress,
                        transactionMeta.ToAddress, long.Parse(transactionMeta.Amount), transactionMeta.IncludeFee);

                transactionBody = new TTransactionBody
                {
                    OperationId = operationId,
                    UnsignedTransaction = unsignedTransaction
                };

                await SaveTransactionBodyAsync(transactionBody);

                _log.Info(nameof(GetUnsignSendTransactionAsync), $"Create new txBody, with id: {operationId}", JObject.FromObject(transactionBody));
            }
            else
            {
                _log.Info(nameof(GetUnsignSendTransactionAsync), $"Return already existing unsigned transaction, with id: {operationId}", 
                    JObject.FromObject(new
                    {
                        operationId,
                        fromAddress,
                        toAddress,
                        amount,
                        assetId,
                        includeFee
                    }));
            }

            return transactionBody.UnsignedTransaction;
        }

        /// </inheritdoc>
        public async Task<bool> IsTransactionAlreadyBroadcastAsync(Guid operationId)
        {
            var txMeta = await GetTransactionMetaAsync(operationId.ToString());

            if (txMeta != null
                && (txMeta.State == TransactionState.Broadcasted
                    || txMeta.State == TransactionState.Confirmed
                    || txMeta.State == TransactionState.Failed
                    || txMeta.State == TransactionState.BlockChainFailed
                    || txMeta.State == TransactionState.Signed))
            {
                return true;
            }

            return false;
        }

        /// </inheritdoc>
        public async Task<TTransactionMeta> GetTransactionMetaAsync(string id)
        {
            return await _transactionMetaRepository.GetAsync(id);
        }

        /// </inheritdoc>
        public async Task<TTransactionBody> GetTransactionBodyByIdAsync(Guid operationId)
        {
            return await _transactionBodyRepository.GetAsync(operationId.ToString());
        }

        /// </inheritdoc>
        public async Task<bool> SaveTransactionMetaAsync(TTransactionMeta transactionMeta)
        {
            return await _transactionMetaRepository.CreateIfNotExistsAsync(transactionMeta);
        }

        /// </inheritdoc>
        public async Task<bool> SaveTransactionBodyAsync(TTransactionBody transactionBody)
        {
            return await _transactionBodyRepository.CreateIfNotExistsAsync(transactionBody);
        }

        public async Task<IEnumerable<Coin>> GetFilteredUnspentOutputsAsync(string address, int confirmationsCount = 0)
        {
            return await Filter(await _blockchainService.GetUnspentOutputsAsync(address, confirmationsCount));
        }

        private async Task<IEnumerable<Coin>> Filter(IList<Coin> coins)
        {
            var spentOutputs = new HashSet<OutPoint>((await _spentOutputRepository.GetSpentOutputs(coins.Select(o => new Output(o.Outpoint))))
                                                                                  .Select(o => new OutPoint(uint256.Parse(o.TransactionHash), o.N)));
            return coins.Where(c => !spentOutputs.Contains(c.Outpoint));
        }

        /// <inheritdoc/>
        public async Task<string> CreateUnsignSendTransactionAsync(string fromAddress, string toAddress, long amount, bool includeFee)
        {
            var builder = new TransactionBuilder();

            var coins = (await GetFilteredUnspentOutputsAsync(fromAddress)).ToList();
            var balance = coins.Select(o => o.Amount).Sum(o => o.Satoshi);

            if (balance > amount &&
                balance - amount < new TxOut(Money.Zero, _blockchainService.ParseAddress(fromAddress)).GetDustThreshold(builder.StandardTransactionPolicy.MinRelayTxFee).Satoshi)
            {
                amount = balance;
            }

            return await SendWithChange(builder, coins, _blockchainService.ParseAddress(toAddress), new Money(balance), new Money(amount), _blockchainService.ParseAddress(fromAddress), includeFee);
        }

        private async Task<string> SendWithChange(TransactionBuilder builder, List<Coin> coins, IDestination destination, Money balance, Money amount, IDestination changeDestination, bool includeFee)
        {
            if (amount.Satoshi <= 0)
                throw new Exception("Amount can't be less or equal to zero");

            builder.AddCoins(coins)
                   .Send(destination, amount)
                   .SetChange(changeDestination);


            var calculatedFee = Money.Satoshis(400000); // TODO use async feeservice instead
            var requiredBalance = amount + (includeFee ? Money.Zero : calculatedFee);

            if (balance < requiredBalance)
                throw new Exception($"The sum of total applicable outputs is less than the required : {requiredBalance} satoshis.");

            if (includeFee)
            {
                if (calculatedFee > amount)
                    throw new Exception($"The sum of total applicable outputs is less than the required fee:{calculatedFee} satoshis.");
                //builder.SubtractFees(); // TODO it needs new version on nbitcoin
                amount = amount - calculatedFee;
            }

            builder.SendFees(calculatedFee);

            var tx = builder.BuildTransaction(false);
            var usedCoins = tx.Inputs.Select(input => coins.First(o => o.Outpoint == input.PrevOut)).ToArray();

            return Serializer.ToString<(Transaction, ICoin[])>((tx, usedCoins));
        }
    }
}
