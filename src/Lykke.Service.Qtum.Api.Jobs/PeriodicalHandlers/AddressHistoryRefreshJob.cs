using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Jobs.PeriodicalHandlers
{
    public class AddressHistoryRefreshJob : TimerPeriod
    {

        private readonly IHistoryService<AddressHistoryEntry, AddressObservation> _historyService;
        private readonly ILog _log;

        public AddressHistoryRefreshJob(TimeSpan period, ILogFactory logFactory, IHistoryService<AddressHistoryEntry, AddressObservation> historyService, string componentName = null) : base(period, logFactory, componentName)
        {
            _historyService = historyService;
            _log = logFactory.CreateLog(this);
        }

        public override async Task Execute()
        {
            try
            {
                _log.Info("Updating address history started");
                await _historyService.UpdateObservedAddressHistoryAsync();
                _log.Info("Updating address history finished");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to update balances");
            }
        }
    }
}
