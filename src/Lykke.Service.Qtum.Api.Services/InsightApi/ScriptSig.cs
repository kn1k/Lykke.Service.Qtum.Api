using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class ScriptSig : IScriptSig
    {
        [JsonProperty("hex")]
        public string Hex { get; set; }

        [JsonProperty("asm")]
        public string Asm { get; set; }

        [JsonProperty("addresses", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Addresses { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public TypeEnum? Type { get; set; }
    }
}
