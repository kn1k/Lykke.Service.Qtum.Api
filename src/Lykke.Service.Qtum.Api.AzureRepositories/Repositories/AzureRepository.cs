using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.Core.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Repositories
{
    public abstract class AzureRepository<T> : IRepository<T> 
        where T : AzureTableEntity, new()
    {
        protected INoSQLTableStorage<T> _tableStorage;
        private readonly ILog _log;

        public AzureRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _tableStorage = AzureTableStorage<T>.Create(connectionStringManager, this.GetType().Name, logFactory);
            _log = logFactory.CreateLog(this);
        }

        public Task UpdateAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return _tableStorage.InsertOrReplaceAsync(item);
        }

        public async Task<bool> CreateIfNotExistsAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return await _tableStorage.CreateIfNotExistsAsync(item);
        }

        public async Task<bool> DeleteIfExistAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return await _tableStorage.DeleteIfExistAsync(item.PartitionKey, item.RowKey);
        }
        
        public async Task<(string continuation, IEnumerable<T> items)> GetAsync(int take = 100, string continuation = null, string partitionKey = null)
        {
            var result = await _tableStorage.GetDataWithContinuationTokenAsync(partitionKey ?? DefaultPartitionKey(), take, continuation);
            return (result.ContinuationToken, result.Entities);
        }

        public async Task<bool> IsExistAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return await _tableStorage.RecordExistsAsync(item);
        }

        /// <summary>
        /// Default PartitionKey for repository
        /// </summary>
        /// <returns></returns>
        public abstract string DefaultPartitionKey();

        public async Task<T> GetAsync(string id, string partitionKey = null)
        {
            return await _tableStorage.GetDataAsync(partitionKey ?? DefaultPartitionKey(), id);
        }
    }
}
