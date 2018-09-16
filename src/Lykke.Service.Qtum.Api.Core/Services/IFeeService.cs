using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    public interface IFeeService
    {
        Task<Money> CalcFeeForTransactionAsync(Transaction tx);
        Task<Money> CalcFeeForTransactionAsync(TransactionBuilder builder);
    }
}
