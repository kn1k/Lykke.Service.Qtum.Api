using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Qtum.Api.Jobs.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class QtumApiJobsSettings
    {
        public DbSettings Db { get; set; }
    }
}
