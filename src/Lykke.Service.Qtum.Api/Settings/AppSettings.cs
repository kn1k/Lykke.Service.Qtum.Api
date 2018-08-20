using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.Qtum.Api.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public QtumApiSettings QtumApiService { get; set; }
    }
}
