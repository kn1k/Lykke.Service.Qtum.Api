using System.Numerics;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;

namespace Lykke.Service.Qtum.Api.Services.InsightApi
{
    public class Utxo : IUtxo
    {
        public string Address { get; set; }
        public string Txid { get; set; }
        public uint Vout { get; set; }
        public string ScriptPubKey { get; set; }
        public decimal Amount { get; set; }
        public string Satoshis { get; set; }
        public bool IsStake { get; set; }
        public long Height { get; set; }
        public long Confirmations { get; set; }
    }
}
