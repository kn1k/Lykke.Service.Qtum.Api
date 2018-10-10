namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IErrorResponse
    {
        string message { get; set; }

        int? code { get; set; }
    }
}
