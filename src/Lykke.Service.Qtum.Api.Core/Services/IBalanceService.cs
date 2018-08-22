using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.Balances;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface IBalanceService<TBalanceObservation, TBalance>
        where TBalanceObservation: IBalanceObservation
        where TBalance : IAddressBalance
    {
        #region BalanceObservation
        
        /// <summary>
        /// Check is address balance already observed
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>true if already observed</returns>
        Task<bool> IsBalanceObservedAsync(TBalanceObservation item);

        /// <summary>
        /// Observe address balance
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> StartBalanceObservationAsync(TBalanceObservation item);

        /// <summary>
        /// Stop observe address balance
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task<bool> StopBalanceObservationAsync(TBalanceObservation item);
        
        #endregion


        #region Balance
        
        /// <summary>
        /// Remove address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task<bool> RemoveBalanceAsync(TBalance item);
        
        /// <summary>
        /// Get balances
        /// </summary>
        /// <param name="take">Amount of balances</param>
        /// <param name="continuation">continuation data</param>
        /// <returns>continuation data and balances</returns>
        Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100, string continuation = null);

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(int take = 100,
            string continuation = null);
        
        /// <summary>
        /// Update observable balances
        /// </summary>
        /// <param name="pageSize">Update page size</param>
        /// <returns><see cref="Task"/></returns>
        Task UpdateBalancesAsync(int pageSize = 10);
       
        #endregion

    }
}
