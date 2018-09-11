using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs
{
    public class Item : IItem
    {
        public string Txid { get; set; }
        public int Version { get; set; }
        public int Locktime { get; set; }
        public Vin[] Vin { get; set; }
        public Vout[] Vout { get; set; }
        public string Blockhash { get; set; }
        public int Blockheight { get; set; }
        public int Confirmations { get; set; }
        public int Time { get; set; }
        public int Blocktime { get; set; }
        public float ValueOut { get; set; }
        public int Size { get; set; }
        public float ValueIn { get; set; }
        public float Fees { get; set; }
    }
}
