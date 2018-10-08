using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Services;
using NBitcoin;

namespace Lykke.Service.Qtum.Api.Services
{
    public class FeeService : IFeeService
    {
        private readonly long _feePerByte;
        private readonly long _minFeeValueSatoshi;
        private readonly long _maxFeeValueSatoshi;

        public FeeService(long feePerByte, long minFeeValueSatoshi, long maxFeeValueSatoshi)
        {
            _feePerByte = feePerByte;
            _minFeeValueSatoshi = minFeeValueSatoshi;
            _maxFeeValueSatoshi = maxFeeValueSatoshi;
        }

        public async Task<Money> CalcFeeForTransactionAsync(Transaction tx)
        {
            var size = tx.ToBytes().Length;

            var feeFromFeeRate = (await GetFeeRate()).GetFee(size);

            return CheckMinMaxThreshold(feeFromFeeRate); 
        }

        public async Task<Money> CalcFeeForTransactionAsync(TransactionBuilder builder)
        {
            var feeRate = await GetFeeRate();

            var feeFromFeeRate = builder.EstimateFees(builder.BuildTransaction(false), feeRate);

            return CheckMinMaxThreshold(feeFromFeeRate);
        }
        
        public async Task<FeeRate> GetFeeRate()
        {
            return new FeeRate(new Money(_feePerByte * 1024, MoneyUnit.Satoshi));
        }

        public long GetMaxFee()
        {
            return _maxFeeValueSatoshi;
        }

        private Money CheckMinMaxThreshold(Money fromFeeRate)
        {
            var min = new Money(_minFeeValueSatoshi, MoneyUnit.Satoshi);
            var max = new Money(_maxFeeValueSatoshi, MoneyUnit.Satoshi);

            return Money.Max(Money.Min(fromFeeRate, max), min);
        }
    }
}
