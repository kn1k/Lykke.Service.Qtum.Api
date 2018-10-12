using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using NBitcoin;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface ITransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation, TOutput>
        where TTransactionBody : ITransactionBody
        where TTransactionMeta : ITransactionMeta
        where TTransactionObservation : ITransactionObservation
        where TOutput: IOutput
    {
        /// <summary>
        /// Check is transaction already broacasted.
        /// </summary>
        /// <param name="operationId">Operation id.</param>
        /// <returns>true if broadcasted.</returns>
        Task<bool> IsTransactionAlreadyBroadcastAsync(Guid operationId);

        /// <summary>
        /// Check is transaction already broacasted.
        /// </summary>
        /// <param name="operationId">Tx meta</param>
        /// <returns>true if broadcasted.</returns>
        bool IsTransactionAlreadyBroadcasted(TTransactionMeta txMeta);

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
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if already observed</returns>
        Task<bool> IsTransactionObservedAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Publish signed transaction to network
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="signedTransaction">Signed transaction</param>
        /// <returns>true if publish, false if already publish</returns>
        Task<bool> BroadcastSignedTransactionAsync(Guid operationId, string signedTransaction);

        /// <summary>
        /// Broadcast transactions to network
        /// </summary>
        /// <param name="minConfirmations">Min confirmation count to transaction complete</param>
        /// <param name="pageSize">Update page size</param>
        /// <returns><see cref="Task"/></returns>
        Task BroadcastSignedTransactionsAsync(long minConfirmations = 20, int pageSize = 10);
        
        /// <summary>
        /// Stop observe trancaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task<bool> RemoveTransactionObservationAsync(TTransactionObservation transactionObservation);
        
        Task<TTransactionMeta> UpdateTransactionBroadcastStatusAsync(Guid operationId, bool check = false);

    }
}
