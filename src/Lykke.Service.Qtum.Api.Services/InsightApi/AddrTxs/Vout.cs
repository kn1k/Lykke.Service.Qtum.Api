using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class Vout : IVout
    {
        public string Value { get; set; }
        public int N { get; set; }
        public IScriptpubkey ScriptPubKey { get; set; }
        public string SpentTxId { get; set; }
        public int? SpentIndex { get; set; }
        public int? SpentHeight { get; set; }
    }
}
