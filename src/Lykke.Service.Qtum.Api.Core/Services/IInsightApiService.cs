using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
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
        
        /// <summary>
        /// Get transaction info by id
        /// </summary>
        /// <param name="txId">Transaction id</param>
        /// <returns><see cref="ITxInfo"/></returns>
        Task<ITxInfo> GetTxByIdAsync(ITxId txId);

        /// <summary>
        /// Get transactions info for specified address
        /// </summary>
        /// <param name="address">Address <see cref="BitcoinAddress"></param>
        /// <param name="from">Paging from setting</param>
        /// <param name="to">Paging to setting</param>
        /// <returns>Transactions info list <see cref="ITxInfo"></returns>
        Task<IAddrTxs> GetAddrTxsAsync(BitcoinAddress address, int from = 0, int to = 50);
    }
}
