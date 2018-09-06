
using Lykke.Service.Qtum.Api.Core.Domain.Balances;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Balances
{
    public interface IAddressBalanceRepository<TAddressBalance> : IRepository<TAddressBalance>
        where TAddressBalance : IAddressBalance
    {
    }
}
