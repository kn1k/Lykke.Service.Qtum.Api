using AzureStorage;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Repositories.TransactionOutputs;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories.TransactionOutputs
{
    public class SpentOutputRepository : AzureRepository<SpentOutputEntity>, ISpentOutputRepository<SpentOutputEntity>
    {
        public SpentOutputRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory) : base(connectionStringManager, logFactory)
        {
        }

        public Task InsertSpentOutputs(Guid operationId, IEnumerable<IOutput> outputs)
        {
            var entities = outputs.Select(o => SpentOutputEntity.Create(o.TransactionHash, o.N, operationId));
            return Task.WhenAll(entities.GroupBy(o => o.PartitionKey).Select((group) => _tableStorage.InsertOrReplaceAsync(group)));
        }

        public async Task<IEnumerable<IOutput>> GetSpentOutputs(IEnumerable<IOutput> outputs)
        {
            return await _tableStorage.GetDataAsync(outputs.Select(o =>
                new Tuple<string, string>(SpentOutputEntity.GeneratePartitionKey(o.TransactionHash), SpentOutputEntity.GenerateRowKey(o.N))));
        }

        public async Task RemoveOldOutputs(DateTime bound)
        {
            string continuation = null;
            IEnumerable<SpentOutputEntity> outputs = null;
            do
            {
                (outputs, continuation) = await _tableStorage.GetDataWithContinuationTokenAsync(100, continuation);
                await Task.WhenAll(outputs.Where(o => o.Timestamp < bound).GroupBy(o => o.PartitionKey).Select(group => _tableStorage.DeleteAsync(group)));
            } while (continuation != null);
        }

        public override string DefaultPartitionKey()
        {
            return null;
        }
    }
}
