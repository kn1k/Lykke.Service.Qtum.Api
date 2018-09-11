using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class RawTx : IRawTx, IErrorResponse
    {
        public string rawtx { get; set; }
        
        public string error { get; set; }
    }
}
