using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.QtumApi.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
