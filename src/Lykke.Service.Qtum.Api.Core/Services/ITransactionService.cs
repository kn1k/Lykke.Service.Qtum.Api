using System;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface ITransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation>
        where TTransactionBody : ITransactionBody
        where TTransactionMeta : ITransactionMeta
        where TTransactionObservation : ITransactionObservation
    {
        /// <summary>
        /// Check is transaction already broacasted.
        /// </summary>
        /// <param name="operationId">Operation id.</param>
        /// <returns>true if broadcasted.</returns>
        Task<bool> IsTransactionAlreadyBroadcastAsync(Guid operationId);

        /// <summary>
        /// Get new or exist unsigned transaction
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="fromAddress">Address from</param>
        /// <param name="toAddress">Address to</param>
        /// <param name="amount">Amount</param>
        /// <param name="assetId">Asset Id</param>
        /// <param name="includeFee">Include fee</param>
        /// <returns>Unsigned transaction context</returns>
        Task<string> GetUnsignSendTransactionAsync(Guid operationId, string fromAddress, string toAddress, string amount, string assetId, bool includeFee);

        /// <summary>
        /// Get transaction meta by operation Id
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns>Transaction meta</returns>
        Task<TTransactionMeta> GetTransactionMetaAsync(string id);

        /// <summary>
        /// Get transaction body by operation id
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Transaction body</returns>
        Task<TTransactionBody> GetTransactionBodyByIdAsync(Guid operationId);

        /// <summary>
        /// Save transaction meta
        /// </summary>
        /// <param name="transactionMeta">Transaction meta</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> SaveTransactionMetaAsync(TTransactionMeta transactionMeta);

        /// <summary>
        /// Save transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> SaveTransactionBodyAsync(TTransactionBody transactionBody);
    }
}
