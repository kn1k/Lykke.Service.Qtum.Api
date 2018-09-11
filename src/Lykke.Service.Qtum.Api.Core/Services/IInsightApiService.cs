using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status;
using NBitcoin;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface IInsightApiService
    {
        /// <summary>
        /// Get insight api status
        /// </summary>
        /// <returns>Status <see cref="IStatus"/></returns>
        Task<IStatus> GetStatusAsync();
        
        /// <summary>
        /// Get address utxo
        /// </summary>
        /// <param name="address">Address <see cref="BitcoinAddress"/></param>
        /// <returns>Address utxo list <see cref="IUtxo"/></returns>
        Task<List<IUtxo>> GetUtxoAsync(BitcoinAddress address);
        
        /// <summary>
        /// Transaction broadcasting
        /// </summary>
        /// <param name="rawtx">Signed transaction as hex string</param>
        /// <returns></returns>
        Task<(ITxId txId, IErrorResponse error)> TxSendAsync(IRawTx rawtx);
        
        Task<ITxInfo> GetTxByIdAsync(ITxId txId);
    }
}
