using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.Status
{
    public class Difficulty : IDifficulty
    {
        public double ProofOfWork { get; set; }
        public double ProofOfStake { get; set; }
    }
}
