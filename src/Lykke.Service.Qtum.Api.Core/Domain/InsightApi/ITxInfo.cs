namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface ITxInfo
    {
        string Txid { get; set; }

        long Version { get; set; }

        long Locktime { get; set; }

        bool Isqrc20Transfer { get; set; }

        IVin[] Vin { get; set; }

        IVout[] Vout { get; set; }

        string Blockhash { get; set; }

        long Blockheight { get; set; }

        long Confirmations { get; set; }

        long Time { get; set; }

        long Blocktime { get; set; }

        double ValueOut { get; set; }

        long Size { get; set; }

        double ValueIn { get; set; }

        long Fees { get; set; }
    }
}
