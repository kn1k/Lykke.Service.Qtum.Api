using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.Core.Domain.Balances;
using Lykke.Service.Qtum.Api.Core.Repositories.Balances;
using Lykke.Service.Qtum.Api.Core.Services;

namespace Lykke.Service.Qtum.Api.Services
{
    public class BalanceService<TBalanceObservation, TBalance> : IBalanceService<TBalanceObservation, TBalance>
        where TBalanceObservation : IBalanceObservation
        where TBalance : IAddressBalance
    {
        private readonly IBalanceObservationRepository<TBalanceObservation> _balanceObservationRepository;
        private readonly IAddressBalanceRepository<TBalance> _addressBalanceRepository;
        
        public BalanceService(ILogFactory logFactory, IBalanceObservationRepository<TBalanceObservation> balanceObservationRepository, IAddressBalanceRepository<TBalance> addressBalanceRepository)
        {
            _balanceObservationRepository = balanceObservationRepository;
            _addressBalanceRepository = addressBalanceRepository;
        }

        #region BalanceObservation
               
        /// <inheritdoc/>
        public async Task<bool> IsBalanceObservedAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.IsExistAsync(item);
        }
        
        /// <inheritdoc/>
        public async Task<bool> StartBalanceObservationAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(item);
        }
        
        /// <inheritdoc/>
        public async Task<bool> StopBalanceObservationAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.DeleteIfExistAsync(item);
        }

        #endregion

        #region Balance

        /// <inheritdoc/>
        public async Task<bool> RemoveBalanceAsync(TBalance item)
        {
            return await _addressBalanceRepository.DeleteIfExistAsync(item);
        }

        /// <inheritdoc/>
        public async Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100, string continuation = null)
        {
            return await _addressBalanceRepository.GetAsync(take, continuation);
        }
        
        /// <inheritdoc/>
        public async Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(int take = 100, string continuation = null)
        {
            return await _balanceObservationRepository.GetAsync(take, continuation);
        }

        /// <inheritdoc/>
        public async Task UpdateBalancesAsync(int pageSize = 10)
        {
            (string continuation, IEnumerable<TBalanceObservation> items) balancesObservation;
            string continuation = null;

            do
            {
                balancesObservation = await this.GetBalancesObservationAsync(pageSize, continuation);

                if (balancesObservation.items.Any())
                {
                    continuation = balancesObservation.continuation;

                    //TODO: implement balance update
                }
                
            } while (continuation != null);
        }
        
        #endregion
    }
}
