using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class Vout : IVout
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("n")]
        public long N { get; set; }

        [JsonProperty("scriptPubKey")]
        [JsonConverter(typeof(ConcreteConverter<ScriptPubKey>))]  
        public IScriptPubKey ScriptPubKey { get; set; }

        [JsonProperty("spentTxId")]
        public object SpentTxId { get; set; }

        [JsonProperty("spentIndex")]
        public object SpentIndex { get; set; }

        [JsonProperty("spentHeight")]
        public object SpentHeight { get; set; }
    }
}
