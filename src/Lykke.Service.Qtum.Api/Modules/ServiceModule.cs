using Autofac;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Services;
using Lykke.Service.Qtum.Api.Settings;
using Lykke.SettingsReader;

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
            
            // Services setup
            builder.RegisterType<AssetService>()
                .As<IAssetService>();
        }
    }
}
