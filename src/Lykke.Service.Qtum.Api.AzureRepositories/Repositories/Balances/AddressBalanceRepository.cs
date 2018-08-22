using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances;
using Lykke.Service.Qtum.Api.Core.Repositories.Balances;
using Lykke.SettingsReader;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Balances
{
    public class AddressBalanceRepository : AzureRepository<AddressBalance>, IAddressBalanceRepository<AddressBalance>
    {
        public AddressBalanceRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory) : base(connectionStringManager, logFactory)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(AddressBalance);
        }
    }
}
