using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class AddrTxs : IAddrTxs
    {
        public int TotalItems { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        
        [JsonConverter(typeof(ConcreteConverter<TxInfo>))]   
        public ITxInfo[] Items { get; set; }
    }
}
