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
        /// Get transaction meta by operation Id
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns>Transaction meta</returns>
        Task<TTransactionMeta> GetTransactionMetaAsync(string id);
        
        /// <summary>
        /// Check is transaction already observed
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if already observed</returns>
        Task<bool> IsTransactionObservedAsync(TTransactionObservation transactionObservation);
        
        /// <summary>
        /// Stop observe trancaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task<bool> RemoveTransactionObservationAsync(TTransactionObservation transactionObservation);
    }
}
