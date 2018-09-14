using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
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

        public TransactionService(ILogFactory logFactory, ITransactionBodyRepository<TTransactionBody> transactionBodyRepository, ITransactionMetaRepository<TTransactionMeta> transactionMetaRepository, ITransactionObservationRepository<TTransactionObservation> transactionObservationRepository, IBlockchainService blockchainService)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
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
                    await _blockchainService.CreateUnsignSendTransactionAsync(transactionMeta.FromAddress,
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
    }
}
