using Lykke.Service.Qtum.Api.Core.Domain.Transactions;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface ITransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation>
        where TTransactionBody : ITransactionBody
        where TTransactionMeta : ITransactionMeta
        where TTransactionObservation : ITransactionObservation
    {
        
    }
}
