using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Qtum.Api.Client 
{
    /// <summary>
    /// Qtum.Api client settings.
    /// </summary>
    public class QtumApiServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
