using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class TxInfo : ITxInfo
    {
        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("locktime")]
        public long Locktime { get; set; }

        [JsonProperty("isqrc20Transfer")]
        public bool Isqrc20Transfer { get; set; }

        [JsonProperty("vin")]
        [JsonConverter(typeof(ConcreteConverter<Vin[]>))]    
        public IVin[] Vin { get; set; }
    
        [JsonProperty("vout")]
        [JsonConverter(typeof(ConcreteConverter<Vout[]>))]    
        public IVout[] Vout { get; set; }

        [JsonProperty("blockhash")]
        public string Blockhash { get; set; }

        [JsonProperty("blockheight")]
        public long Blockheight { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("blocktime")]
        public long Blocktime { get; set; }

        [JsonProperty("valueOut")]
        public double ValueOut { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("valueIn")]
        public double ValueIn { get; set; }

        [JsonProperty("fees")]
        public long Fees { get; set; }
    }
}
