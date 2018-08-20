using Lykke.HttpClientGenerator;

namespace Lykke.Service.Qtum.Api.Client
{
    /// <summary>
    /// Qtum.Api API aggregating interface.
    /// </summary>
    public class QtumApiClient : IQtumApiClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to Qtum.Api Api.</summary>
        public IQtumApiApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public QtumApiClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IQtumApiApi>();
        }
    }
}
