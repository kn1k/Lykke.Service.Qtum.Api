using NBitcoin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface ITransactionOutputsService
    {
        /// <summary>
        /// Get unspent outputs for the address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="confirmationsCount"></param>
        /// <returns></returns>
        Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int confirmationsCount = 0);
    }
}
