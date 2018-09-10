using Lykke.Service.Qtum.Api.Core.Domain.Balances;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Balances
{
    public interface IBalanceObservationRepository<TBalanceObservation> : IRepository<TBalanceObservation>
        where TBalanceObservation : IBalanceObservation
    {

    }
}
