using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs
{
    public interface IAddrTxs
    {
        int TotalItems { get; set; }
        int From { get; set; }
        int To { get; set; }
        ITxInfo[] Items { get; set; }
    }
}
