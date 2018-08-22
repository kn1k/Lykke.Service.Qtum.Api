using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances;
using Lykke.Service.Qtum.Api.Core.Repositories.Balances;
using Lykke.SettingsReader;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Balances
{
    public class BalanceObservationRepository : AzureRepository<BalanceObservation>, IBalanceObservationRepository<BalanceObservation>
    {
        public BalanceObservationRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory) : base(connectionStringManager, logFactory)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(BalanceObservation);
        }
    }
}
