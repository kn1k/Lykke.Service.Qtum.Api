using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses;
using Lykke.Service.Qtum.Api.Core.Repositories.Addresses;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Addresses
{
    public class AddressObservationRepository : AzureRepository<AddressObservation>, IAddressObservationRepository<AddressObservation>
    {
        public AddressObservationRepository(IReloadingManager<string> connectionStringManager, ILogFactory log) : base(connectionStringManager, log)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(AddressObservation);
        }
    }
}
