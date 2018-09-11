using Lykke.Service.Qtum.Api.Core.Domain.Transactions;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Transactions
{
    public interface ITransactionObservationRepository<TTransactionObservation> : IRepository<TTransactionObservation>
        where TTransactionObservation : ITransactionObservation
    {
        
    }
}
