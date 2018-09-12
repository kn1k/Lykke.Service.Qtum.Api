using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs
{
    public interface IScriptpubkey
    {
        string Hex { get; set; }
        string Asm { get; set; }
        string[] Addresses { get; set; }
        string Type { get; set; }
    }
}
