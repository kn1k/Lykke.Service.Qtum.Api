using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class Scriptpubkey : IScriptpubkey
    {
        public string Hex { get; set; }
        public string Asm { get; set; }
        public string[] Addresses { get; set; }
        public string Type { get; set; }
    }
}
