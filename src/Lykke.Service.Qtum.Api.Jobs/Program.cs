using Lykke.Sdk;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Jobs
{
    internal sealed class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            await LykkeStarter.Start<Startup>(true, 5001);
#else
            await LykkeStarter.Start<Startup>(false);
#endif
        }
    }
}
