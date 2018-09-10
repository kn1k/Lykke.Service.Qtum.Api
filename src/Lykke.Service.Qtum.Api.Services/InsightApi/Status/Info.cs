using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.Status
{
    public class Info : IInfo
    {
        public long Version { get; set; }
        public long Protocolversion { get; set; }
        public long Walletversion { get; set; }
        public long Balance { get; set; }
        public long Blocks { get; set; }
        public long Timeoffset { get; set; }
        public long Connections { get; set; }
        public string Proxy { get; set; }
        [JsonConverter(typeof(ConcreteConverter<Difficulty>))]    
        public IDifficulty Difficulty { get; set; }
        public bool Testnet { get; set; }
        public long Keypoololdest { get; set; }
        public long Keypoolsize { get; set; }
        public long Paytxfee { get; set; }
        public double Relayfee { get; set; }
        public string Errors { get; set; }
        public string Network { get; set; }
        public long Reward { get; set; }
    }
}
