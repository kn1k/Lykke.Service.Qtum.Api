using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.QtumApi.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class QtumApiSettings
    {
        public DbSettings Db { get; set; }
    }
}
