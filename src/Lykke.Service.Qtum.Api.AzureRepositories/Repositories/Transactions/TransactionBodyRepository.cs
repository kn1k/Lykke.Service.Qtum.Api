using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.SettingsReader;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Transactions
{
    public class TransactionBodyRepository  : AzureRepository<TransactionBody>, ITransactionBodyRepository<TransactionBody>
    {
        public TransactionBodyRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory) : base(connectionStringManager, logFactory)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(TransactionBody);
        }
       
    }
}
