using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class TxId : ITxId
    {
        public string txid { get; set; }
    }

    public class TxResult
    {
        public string result { get; set; }
        public Error error { get; set; }
        public string id { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
    }

}
