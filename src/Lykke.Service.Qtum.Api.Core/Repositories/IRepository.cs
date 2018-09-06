using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Repositories
{
    public interface IRepository<T>
    {
        Task<bool> CreateIfNotExistsAsync(T item);

        Task<bool> DeleteIfExistAsync(T item);

        Task<bool> IsExistAsync(T item);

        Task<(string continuation, IEnumerable<T> items)> GetAsync(int take = 100, string continuation = null, string partitionKey = null);

        Task<T> GetAsync(string id, string partitionKey = null);

        Task UpdateAsync(T item);
    }
}
