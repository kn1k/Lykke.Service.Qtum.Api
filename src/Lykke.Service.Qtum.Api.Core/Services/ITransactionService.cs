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

        /// <summary>
        /// Get unspent outputs for the address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="confirmationsCount"></param>
        /// <returns></returns>
        Task<IEnumerable<Coin>> GetFilteredUnspentOutputsAsync(string address, int confirmationsCount = 0);

        /// <summary>
        /// Build unsined send transaction
        /// </summary>
        /// <param name="fromAddress">Address from</param>
        /// <param name="toAddress">Address to</param>
        /// <param name="amount">Amount</param>
        /// <param name="includeFee">Flag indicates that transaction should incude fee</param>
        /// <returns>Unsined transaction</returns>
        Task<string> CreateUnsignSendTransactionAsync(string fromAddress, string toAddress, long amount, bool includeFee);
        
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
        
        Task<Dictionary<string, string>> GetTransactionInputsAsync(string txId);
        
        Task<Dictionary<string, string>> GetTransactionOutputsAsync(string txId);

    }
}
