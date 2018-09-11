using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class ErrorResponse : IErrorResponse
    {
        public string error { get; set; }
    }
}
