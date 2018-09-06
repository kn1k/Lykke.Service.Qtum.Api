using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.Status
{
    public class Status : IStatus
    {
        [JsonConverter(typeof(ConcreteConverter<Info>))]    
        public IInfo Info { get; set; }
    }
}
