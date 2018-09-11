using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class Vin : IVin
    {
        public string Txid { get; set; }
        public int Vout { get; set; }
        public long Sequence { get; set; }
        public int N { get; set; }
        public IScriptsig ScriptSig { get; set; }
        public string Addr { get; set; }
        public long ValueSat { get; set; }
        public float Value { get; set; }
        public object DoubleSpentTxID { get; set; }
    }
}
