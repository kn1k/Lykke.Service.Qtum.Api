using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;

namespace Lykke.Service.Qtum.Api.Jobs.PeriodicalHandlers
{
    public class BroadcastJob: TimerPeriod
    {
        private readonly ILog _log;
        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation>
            _transactionService;
        
        public BroadcastJob(TimeSpan period, ILogFactory logFactory, ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> transactionService, string componentName = null) : base(period, logFactory, componentName)
        {
            _transactionService = transactionService;
            _log = logFactory.CreateLog(this);
        }
        
        public override async Task Execute()
        {
            try
            {
                _log.Info("BroadcastJob start");
                await _transactionService.BroadcastSignedTransactionsAsync();
                _log.Info("BroadcastJob finished");
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to update Broadcast info");
            }

        } 
    }
}
