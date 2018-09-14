using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Helpers;
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

        public TransactionService(ILogFactory logFactory,
            ITransactionBodyRepository<TTransactionBody> transactionBodyRepository,
            ITransactionMetaRepository<TTransactionMeta> transactionMetaRepository,
            ITransactionObservationRepository<TTransactionObservation> transactionObservationRepository,
            ISpentOutputRepository<TOutput> spentOutputRepository,
            IBlockchainService blockchainService)
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

        /// <inheritdoc/>
        public async Task<bool> IsTransactionObservedAsync(TTransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.IsExistAsync(transactionObservation);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveTransactionObservationAsync(TTransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.DeleteIfExistAsync(transactionObservation);
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> GetTransactionInputsAsync(string txId)
        {
            return (await _blockchainService.GetTransactionInfoByIdAsync(txId)).Vin.ToDictionary(x => x.Addr, x => x.Value.ToString());
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> GetTransactionOutputsAsync(string txId)
        {
            return (await _blockchainService.GetTransactionInfoByIdAsync(txId)).Vout.ToDictionary(x => x.ScriptPubKey.Addresses.FirstOrDefault(), x => x.Value.ToString());
        }

        /// <summary>
        /// Update transaction meta
        /// </summary>
        /// <param name="transactionMeta">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        private Task UpdateTransactionMeta(TTransactionMeta transactionMeta)
        {
            return _transactionMetaRepository.UpdateAsync(transactionMeta);
        }

        /// <summary>
        /// Observe tansaction
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if created, false if existed before</returns>
        private async Task<bool> CreateObservationAsync(TTransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.CreateIfNotExistsAsync(transactionObservation);
        }

        /// <summary>
        /// Get observed transaction
        /// </summary>
        /// <param name="pageSize">Abount of transaction observation</param>
        /// <param name="continuation">ontinuation data</param>
        /// <returns>ontinuation data and transaction observation</returns>
        public async Task<(string continuation, IEnumerable<TTransactionObservation> items)>
            GetTransactionObservationAsync(int pageSize, string continuation)
        {
            return await _transactionObservationRepository.GetAsync(pageSize, continuation);
        }
        
        /// <inheritdoc/>
        public async Task<bool> BroadcastSignedTransactionAsync(Guid operationId, string signedTransaction)
        {
            var txMeta = await GetTransactionMetaAsync(operationId.ToString());

            if (await IsTransactionAlreadyBroadcastAsync(operationId))
            {
                _log.Info(nameof(BroadcastSignedTransactionAsync),
                    JObject.FromObject(txMeta).ToString(),
                    "TxMeta already broadcasted or failed, with id: {operationId}");
                return false;
            }

            if (txMeta == null)
            {
                txMeta = new TTransactionMeta
                {
                    OperationId = operationId
                };
            }

            TTransactionBody transactionBody = await GetTransactionBodyByIdAsync(operationId);
            if (transactionBody == null)
            {
                transactionBody = new TTransactionBody
                {
                    OperationId = operationId
                };
            }

            transactionBody.SignedTransaction = signedTransaction;

            await UpdateTransactionBodyAsync(transactionBody);

            txMeta.State = TransactionState.Signed;
            txMeta.BroadcastTimestamp = DateTime.Now;
            await UpdateTransactionMeta(txMeta);

            TTransactionObservation transactionObservation = new TTransactionObservation
            {
                OperationId = operationId
            };

            await CreateObservationAsync(transactionObservation);

            _log.Info(nameof(BroadcastSignedTransactionAsync),
                JObject.FromObject(transactionObservation).ToString(),
                $"Observe new transaction, with id: {operationId}");

            return true;
        }

        /// <inheritdoc/>
        public async Task BroadcastSignedTransactionsAsync(long minConfirmations = 20, int pageSize = 10)
        {
            string continuation = null;
            do
            {
                var transactionObservations = await GetTransactionObservationAsync(pageSize, continuation);

                foreach (var transactionObservation in transactionObservations.items)
                {
                    try
                    {
                        var txMeta =
                            await GetTransactionMetaAsync(transactionObservation.OperationId
                                .ToString());

                        if (txMeta.State == TransactionState.Signed)
                        {
                            var txBody =
                                await GetTransactionBodyByIdAsync(transactionObservation.OperationId);
                            var broadcactResult =
                                await _blockchainService.BroadcastSignedTransactionAsync(txBody.SignedTransaction);

                            if (broadcactResult.error != null)
                            {
                                txMeta.Error = broadcactResult.error;
                                txMeta.State = TransactionState.Failed;
                            }
                            else
                            {
                                txMeta.TxId = broadcactResult.txId;
                                
                                var txInfo = await _blockchainService.GetTransactionInfoByIdAsync(txMeta.TxId);

                                if (txInfo != null)
                                {
                                    if (txInfo.Confirmations > minConfirmations)
                                    {
                                        txMeta.State = TransactionState.Confirmed;
                                        txMeta.Hash = txInfo.Blockhash;
                                        txMeta.CompleteTimestamp = UnixTimeHelper.UnixTimeStampToDateTime(txInfo.Blocktime);
                                        txMeta.BlockCount = txInfo.Blockheight;
                                    }
                                }
                                else
                                {
                                    txMeta.Error = $"Tx not found by id :{txMeta.TxId}";
                                    txMeta.State = TransactionState.Failed;
                                }
                            }
                        }
                        
                        await UpdateTransactionMeta(txMeta);
                    }
                    catch (Exception e)
                    {
                        _log.Error($"failed to update tx info for operation: {transactionObservation.OperationId}", e);
                    }

                }

                continuation = transactionObservations.continuation;
            } while (continuation != null);
        }

        /// <summary>
        /// Update transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        private Task UpdateTransactionBodyAsync(TTransactionBody transactionBody)
        {
            return _transactionBodyRepository.UpdateAsync(transactionBody);
        }
    }
}
