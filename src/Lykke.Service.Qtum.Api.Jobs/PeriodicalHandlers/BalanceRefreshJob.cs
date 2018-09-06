using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances;
using Lykke.Service.Qtum.Api.Core.Services;

namespace Lykke.Service.Qtum.Api.Jobs.PeriodicalHandlers
{
    public class BalanceRefreshJob: TimerPeriod
    {
        
        private readonly IBalanceService<BalanceObservation, AddressBalance> _balanceService;
        private readonly ILog _log;
        
        public BalanceRefreshJob(TimeSpan period, ILogFactory logFactory, IBalanceService<BalanceObservation, AddressBalance> balanceService, string componentName = null) : base(period, logFactory, componentName)
        {
            _balanceService = balanceService;
            _log = logFactory.CreateLog(this);
        }
        
        public override async Task Execute()
        {
            try
            {
                _log.Info("Update balances start");
                await _balanceService.UpdateBalancesAsync();
                _log.Info("Update balances finished");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to update balances");
            }

        } 
    }
}
