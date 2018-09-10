using Lykke.Service.Qtum.Api.Core.Domain.Transactions;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Transactions
{
    public interface ITransactionMetaRepository<TTransactionMeta> : IRepository<TTransactionMeta>
        where TTransactionMeta : ITransactionMeta
    {
        
    }
}
