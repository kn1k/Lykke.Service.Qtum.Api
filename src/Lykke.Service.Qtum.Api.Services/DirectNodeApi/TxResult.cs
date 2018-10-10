using Lykke.Service.Qtum.Api.Core.Domain.DirectNodeApi;
using Lykke.Service.Qtum.Api.Services.InsightApi.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Services.DirectNodeApi
{
    public class TxResult : ITxResult
    {
        public string result { get; set; }
        [JsonConverter(typeof(ConcreteConverter<TxError>))]
        public ITxError error { get; set; }
        public string id { get; set; }
    }
}
