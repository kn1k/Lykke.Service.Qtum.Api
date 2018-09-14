using System;

namespace Lykke.Service.Qtum.Api.Core.Helpers
{
    public static class UnixTimeHelper
    {
        private static DateTime DtDateTime { get => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch            
            return DtDateTime.AddSeconds(unixTimeStamp);
        }

    }
}
