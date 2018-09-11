using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs
{
    public interface IItem
    {
        string Txid { get; set; }
        int Version { get; set; }
        int Locktime { get; set; }
        Vin[] Vin { get; set; }
        Vout[] Vout { get; set; }
        string Blockhash { get; set; }
        int Blockheight { get; set; }
        int Confirmations { get; set; }
        int Time { get; set; }
        int Blocktime { get; set; }
        float ValueOut { get; set; }
        int Size { get; set; }
        float ValueIn { get; set; }
        float Fees { get; set; }
    }
}
