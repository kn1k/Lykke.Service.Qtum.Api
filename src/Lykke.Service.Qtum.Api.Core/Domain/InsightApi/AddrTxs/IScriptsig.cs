using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs
{
    public interface IScriptsig
    {
        string Hex { get; set; }
        string Asm { get; set; }
    }
}
