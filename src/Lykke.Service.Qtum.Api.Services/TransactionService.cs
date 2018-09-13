using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Lykke.Service.Qtum.Api.Core.Helpers;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;
using Newtonsoft.Json.Linq;

namespace Lykke.Service.Qtum.Api.Services
{
    public class TransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation> : ITransactionService<
        TTransactionBody, TTransactionMeta, TTransactionObservation>
        where TTransactionBody : ITransactionBody, new()
        where TTransactionMeta : ITransactionMeta, new()
        where TTransactionObservation : ITransactionObservation, new()
    {
        private readonly ILog _log;
        private readonly ITransactionBodyRepository<TTransactionBody> _transactionBodyRepository;
        private readonly ITransactionMetaRepository<TTransactionMeta> _transactionMetaRepository;
        private readonly ITransactionObservationRepository<TTransactionObservation> _transactionObservationRepository;
        private readonly IBlockchainService _blockchainService;

        public TransactionService(ILogFactory logFactory,
            ITransactionBodyRepository<TTransactionBody> transactionBodyRepository,
            ITransactionMetaRepository<TTransactionMeta> transactionMetaRepository,
            ITransactionObservationRepository<TTransactionObservation> transactionObservationRepository,
            IBlockchainService blockchainService)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
            _blockchainService = blockchainService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Get transaction body by operation id
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Transaction body</returns>
        private async Task<TTransactionBody> GetTransactionBodyByIdAsync(Guid operationId)
        {
            return await _transactionBodyRepository.GetAsync(operationId.ToString());
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
                        }
                    }

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

                    await UpdateTransactionMeta(txMeta);
                }

                continuation = transactionObservations.continuation;
            } while (continuation != null);
        }

        /// <summary>
        /// Check is transaction already broacasted.
        /// </summary>
        /// <param name="operationId">Operation id.</param>
        /// <returns>true if broadcasted.</returns>
        private async Task<bool> IsTransactionAlreadyBroadcastAsync(Guid operationId)
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

        /// <summary>
        /// Update transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        private Task UpdateTransactionBodyAsync(TTransactionBody transactionBody)
        {
            return _transactionBodyRepository.UpdateAsync(transactionBody);
        }

        /// <summary>
        /// Get transaction meta by operation Id
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns>Transaction meta</returns>
        public async Task<TTransactionMeta> GetTransactionMetaAsync(string id)
        {
            return await _transactionMetaRepository.GetAsync(id);
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
    }
}
