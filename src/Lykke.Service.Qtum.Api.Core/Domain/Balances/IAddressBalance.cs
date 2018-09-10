using System;

namespace Lykke.Service.Qtum.Api.Core.Domain.Balances
{
    public interface IAddressBalance
    {
        string Address { get; set; }

        string AssetId { get; }

        string Balance { get; set; }

        Int64 Block { get; set; }
    }
}
