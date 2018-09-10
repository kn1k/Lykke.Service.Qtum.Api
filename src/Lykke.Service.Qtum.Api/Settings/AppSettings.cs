using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.Qtum.Api.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public QtumApiSettings QtumApiService { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public string Network{ get; set; }
    }
}
