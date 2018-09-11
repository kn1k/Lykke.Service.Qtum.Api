using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs
{
    public interface IVout
    {
        string Value { get; set; }
        int N { get; set; }
        IScriptpubkey ScriptPubKey { get; set; }
        string SpentTxId { get; set; }
        int? SpentIndex { get; set; }
        int? SpentHeight { get; set; }
    }
}
