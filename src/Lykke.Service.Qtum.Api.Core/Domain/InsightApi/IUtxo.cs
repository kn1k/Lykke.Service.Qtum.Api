using System.Numerics;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IUtxo
    {
        string Address { get; set; }

        string Txid { get; set; }

        uint Vout { get; set; }

        string ScriptPubKey { get; set; }

        decimal Amount { get; set; }

        string Satoshis { get; set; }

        bool IsStake { get; set; }

        long Height { get; set; }

        long Confirmations { get; set; }
    }
}
