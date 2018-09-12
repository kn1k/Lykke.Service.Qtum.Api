using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class AddrTxs : IAddrTxs
    {
        public int TotalItems { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public IItem[] Items { get; set; }
    }
}
