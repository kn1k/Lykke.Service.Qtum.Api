using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class ErrorResponse : IErrorResponse
    {
        public string message { get; set; }

        public int? code { get; set; }
    }
}
