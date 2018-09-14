using Autofac;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions;
using Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Addresses;
using Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Balances;
using Lykke.Service.Qtum.Api.AzureRepositories.Repositories.TransactionOutputs;
using Lykke.Service.Qtum.Api.AzureRepositories.Repositories.Transactions;
using Lykke.Service.Qtum.Api.Core.Helpers;
using Lykke.Service.Qtum.Api.Core.Repositories.Addresses;
using Lykke.Service.Qtum.Api.Core.Repositories.Balances;
using Lykke.Service.Qtum.Api.Core.Repositories.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Services;
using Lykke.Service.Qtum.Api.Settings;
using Lykke.SettingsReader;
using NBitcoin;
using NBitcoin.Qtum;

namespace Lykke.Service.Qtum.Api.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            // Network setup
            QtumNetworks.Register();
            builder.RegisterInstance(Network.GetNetwork(_appSettings.Nested(s => s.Network).CurrentValue))
                .As<Network>();

            // CoinConverter            
            builder.RegisterType<CoinConverter>()
                .As<CoinConverter>();

            // Repositories setup
            var dataConnString = _appSettings.Nested(s => s.QtumApiService.Db.DataConnString);

            builder.RegisterType<BalanceObservationRepository>()
                .As<IBalanceObservationRepository<BalanceObservation>>()
                .WithParameter(TypedParameter.From(dataConnString));

            builder.RegisterType<AddressBalanceRepository>()
                .As<IAddressBalanceRepository<AddressBalance>>()
                .WithParameter(TypedParameter.From(dataConnString));

            builder.RegisterType<TransactionBodyRepository>()
                .As<ITransactionBodyRepository<TransactionBody>>()
                .WithParameter(TypedParameter.From(dataConnString));

            builder.RegisterType<TransactionMetaRepository>()
                .As<ITransactionMetaRepository<TransactionMeta>>()
                .WithParameter(TypedParameter.From(dataConnString));

            builder.RegisterType<TransactionObservationRepository>()
                .As<ITransactionObservationRepository<TransactionObservation>>()
                .WithParameter(TypedParameter.From(dataConnString));

            builder.RegisterType<AddressHistoryEntryRepository>()
                .As<IAddressHistoryEntryRepository<AddressHistoryEntry>>()
                .WithParameter(TypedParameter.From(dataConnString));

            builder.RegisterType<AddressObservationRepository>()
                .As<IAddressObservationRepository<AddressObservation>>()
                .WithParameter(TypedParameter.From(dataConnString));
            
            builder.RegisterType<SpentOutputRepository>()
                .As<ISpentOutputRepository<SpentOutputEntity>>()
                .WithParameter(TypedParameter.From(dataConnString));

            // Services setup
            builder.RegisterType<AssetService>()
                .As<IAssetService>()
                .SingleInstance();

            builder.RegisterType<BlockchainService>()
                .As<IBlockchainService>()
                .WithParameter("networkType", _appSettings.Nested(s => s.Network).CurrentValue);

            builder.RegisterType<BalanceService<BalanceObservation, AddressBalance>>()
                .As<IBalanceService<BalanceObservation, AddressBalance>>();
            
            builder.RegisterType<TransactionService<TransactionBody, TransactionMeta, TransactionObservation>>()
                .As<ITransactionService<TransactionBody, TransactionMeta, TransactionObservation>>();

            builder.RegisterType<HistoryService<AddressHistoryEntry, AddressObservation>>()
                .As<IHistoryService<AddressHistoryEntry, AddressObservation>>();

            builder.RegisterType<QtumInsightApi>()
                .As<IInsightApiService>()
                .WithParameter(
                    TypedParameter.From(_appSettings.Nested(s => s.ExternalApi.QtumInsightApi).CurrentValue));
            
            builder.RegisterType<FeeService>()
                .As<IFeeService>()
                .WithParameter("feePerByte", TypedParameter.From(_appSettings.Nested(s => s.FeeSettings.FeePerByte).CurrentValue))
                .WithParameter("minFeeValueSatoshi", TypedParameter.From(_appSettings.Nested(s => s.FeeSettings.MinFeeValueSatoshi).CurrentValue))
                .WithParameter("maxFeeValueSatoshi",TypedParameter.From(_appSettings.Nested(s => s.FeeSettings.MaxFeeValueSatoshi).CurrentValue));
        }
    }
}
