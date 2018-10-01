using System;

namespace Lykke.Service.Qtum.Api.Core.Helpers
{
    public static class UnixTimeHelper
    {
        private static DateTime _dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0,System.DateTimeKind.Utc);

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            _dtDateTime = _dtDateTime.AddSeconds(unixTimeStamp);
            return _dtDateTime;
        }

        public static double DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (dateTime - _dtDateTime).TotalSeconds;
        }
    }
}
