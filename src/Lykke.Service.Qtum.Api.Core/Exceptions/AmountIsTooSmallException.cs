using System;

namespace Lykke.Service.Qtum.Api.Core.Exceptions
{
    public class AmountIsTooSmallException: Exception
    {
        public AmountIsTooSmallException(string message) : base(message)
        {
        }
    }
}
