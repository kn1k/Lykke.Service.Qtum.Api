namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status
{
    public interface IDifficulty
    {
        double ProofOfWork { get; set; }

        double ProofOfStake { get; set; }
    }
}
