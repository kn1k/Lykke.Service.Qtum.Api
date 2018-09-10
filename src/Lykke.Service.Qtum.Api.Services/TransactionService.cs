using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;

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

        public TransactionService(ILogFactory logFactory, ITransactionBodyRepository<TTransactionBody> transactionBodyRepository, ITransactionMetaRepository<TTransactionMeta> transactionMetaRepository, ITransactionObservationRepository<TTransactionObservation> transactionObservationRepository)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
            _log = logFactory.CreateLog(this);
        }
    }
}
