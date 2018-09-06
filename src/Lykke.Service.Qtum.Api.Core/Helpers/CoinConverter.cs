using System;
using System.Numerics;
using Lykke.Service.Qtum.Api.Core.Services;

namespace Lykke.Service.Qtum.Api.Core.Helpers
{
    public class CoinConverter
    {
        private readonly int _accuracy;

        public CoinConverter(IAssetService assetService)
        {
            _accuracy = assetService.GetQtumAsset().Accuracy;
        }

        public string LykkeQtumToQtum(string lykkeQtum)
        {
            var result = BigInteger.TryParse(lykkeQtum, out var lykkeQtumParsed);

            if (result)
            {
                return (lykkeQtumParsed * BigInteger.Pow(10, _accuracy)).ToString();
            } else
            {
                throw new ArgumentException("Invalid lykkeQtum value, must be BigInteger");
            }

        }

        public string QtumToLykkeQtum(string qtum)
        {
            var result = BigInteger.TryParse(qtum, out var qtumParsed);

            if (result)
            {
                return (qtumParsed / BigInteger.Pow(10, _accuracy)).ToString();
            }
            else
            {
                throw new ArgumentException("Invalid qtum value, must be BigInteger");
            }

        }
    }
}
