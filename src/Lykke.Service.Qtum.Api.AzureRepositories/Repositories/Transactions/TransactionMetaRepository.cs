using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.SettingsReader;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Transactions
{
    public class TransactionMetaRepository : AzureRepository<TransactionMeta>, ITransactionMetaRepository<TransactionMeta>
    {
        public TransactionMetaRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory) : base(connectionStringManager, logFactory)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(TransactionMeta);
        } 
    }
}
