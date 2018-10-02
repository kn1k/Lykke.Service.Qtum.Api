using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class Vin : IVin

    {
        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("vout")]
        public long Vout { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }

        [JsonProperty("n")]
        public long N { get; set; }

        [JsonProperty("scriptSig")]
        [JsonConverter(typeof(ConcreteConverter<ScriptSig>))]    
        public IScriptSig ScriptSig { get; set; }

        [JsonProperty("addr")]
        public string Addr { get; set; }

        [JsonProperty("valueSat")]
        public long ValueSat { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("doubleSpentTxID")]
        public object DoubleSpentTxId { get; set; }
    }
}
