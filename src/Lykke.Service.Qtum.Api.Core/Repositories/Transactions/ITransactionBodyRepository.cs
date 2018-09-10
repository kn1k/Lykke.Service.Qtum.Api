using Lykke.Service.Qtum.Api.Core.Domain.Transactions;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Transactions
{
    public interface ITransactionBodyRepository<TTransactionBody> : IRepository<TTransactionBody>
        where TTransactionBody : ITransactionBody
    {
        
    }
}
