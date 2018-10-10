using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface IDirectNodeApiService
    {
        /// <summary>
        /// Broadcast transaction to the node API directly.
        /// </summary>
        /// <param name="rawTx"></param>
        /// <returns></returns>
        Task<(ITxId txId, IErrorResponse error)> TxSendAsync(IRawTx rawTx);
    }
}
