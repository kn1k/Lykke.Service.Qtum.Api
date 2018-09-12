using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class Scriptsig : IScriptsig
    {
        public string Hex { get; set; }
        public string Asm { get; set; }
    }
}
