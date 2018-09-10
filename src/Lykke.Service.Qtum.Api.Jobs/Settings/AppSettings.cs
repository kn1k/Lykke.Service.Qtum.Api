using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.Qtum.Api.Jobs.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public QtumApiJobsSettings QtumApiJobsService { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public string Network{ get; set; }
        
        public ExternalApi ExternalApi { get; set; }
        
    }
}
