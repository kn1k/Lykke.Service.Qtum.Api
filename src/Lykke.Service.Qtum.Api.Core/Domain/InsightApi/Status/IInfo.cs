namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status
{
    public interface IInfo
    {
        long Version { get; set; }

        long Protocolversion { get; set; }

        long Walletversion { get; set; }

        long Balance { get; set; }

        long Blocks { get; set; }

        long Timeoffset { get; set; }

        long Connections { get; set; }

        string Proxy { get; set; }

        IDifficulty Difficulty { get; set; }

        bool Testnet { get; set; }

        long Keypoololdest { get; set; }

        long Keypoolsize { get; set; }

        long Paytxfee { get; set; }

        double Relayfee { get; set; }

        string Errors { get; set; }

        string Network { get; set; }

        long Reward { get; set; }
    }
}
