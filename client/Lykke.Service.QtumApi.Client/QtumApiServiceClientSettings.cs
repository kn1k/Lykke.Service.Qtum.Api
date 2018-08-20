using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.QtumApi.Client 
{
    /// <summary>
    /// QtumApi client settings.
    /// </summary>
    public class QtumApiServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
