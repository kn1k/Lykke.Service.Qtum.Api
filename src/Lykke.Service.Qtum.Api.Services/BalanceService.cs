using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        where TBalance : IAddressBalance, new()
    {
        private readonly IBalanceObservationRepository<TBalanceObservation> _balanceObservationRepository;
        private readonly IAddressBalanceRepository<TBalance> _addressBalanceRepository;
        private readonly IBlockchainService _blockchainService;
        private readonly ILog _log;

        public BalanceService(ILogFactory logFactory,
            IBalanceObservationRepository<TBalanceObservation> balanceObservationRepository,
            IAddressBalanceRepository<TBalance> addressBalanceRepository, IBlockchainService blockchainService)
        {
            _balanceObservationRepository = balanceObservationRepository;
            _addressBalanceRepository = addressBalanceRepository;
            _blockchainService = blockchainService;
            _log = logFactory.CreateLog(this);
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
        public async Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100,
            string continuation = null)
        {
            return await _addressBalanceRepository.GetAsync(take, continuation);
        }

        /// <inheritdoc/>
        public async Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(
            int take = 100, string continuation = null)
        {
            return await _balanceObservationRepository.GetAsync(take, continuation);
        }

        /// <inheritdoc/>
        public async Task<bool> IsBalanceExistAsync(TBalance item)
        {
            return await _addressBalanceRepository.IsExistAsync(item);
        }

        /// <inheritdoc/>
        public Task UpdateBalance(TBalance item)
        {
            return _addressBalanceRepository.UpdateAsync(item);
        }

        /// <inheritdoc/>
        public async Task<bool> AddBalance(TBalance item)
        {
            return await _addressBalanceRepository.CreateIfNotExistsAsync(item);
        }

        #endregion

        #region Periodical

        /// <inheritdoc/>
        public async Task UpdateBalancesAsync(int pageSize = 10)
        {
            (string continuation, IEnumerable<TBalanceObservation> items) balancesObservation;
            string continuation = null;

            do
            {
                balancesObservation = await GetBalancesObservationAsync(pageSize, continuation);

                if (balancesObservation.items.Any())
                {
                    continuation = balancesObservation.continuation;

                    foreach (var observedAddress in balancesObservation.items)
                    {
                        try
                        {
                            var balance =
                                await _blockchainService.GetAddressBalanceAsync(
                                    _blockchainService.ParseAddress(observedAddress.Address));
                            var addressBalance = new TBalance
                            {
                                Address = observedAddress.Address,
                                Balance = balance.ToString(),
                                Block = await _blockchainService.GetBlockCountAsync()
                            };

                            if (await IsBalanceExistAsync(addressBalance))
                            {
                                if (balance > 0)
                                {
                                    await UpdateBalance(addressBalance);
                                }
                                else
                                {
                                    await RemoveBalanceAsync(addressBalance);
                                }
                            }
                            else
                            {
                                if (balance > 0)
                                {
                                    await AddBalance(addressBalance);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex, $"Failed to update balance for address: {observedAddress.Address}");
                        }
                    }
                }
            } while (continuation != null);
        }

        #endregion
    }
}
