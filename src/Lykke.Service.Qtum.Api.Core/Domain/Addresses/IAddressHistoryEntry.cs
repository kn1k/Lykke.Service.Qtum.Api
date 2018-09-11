using System;

namespace Lykke.Service.Qtum.Api.Core.Domain.Addresses
{
    public interface IAddressHistoryEntry
    {
        string FromAddress { get; set; }

        string ToAddress { get; set; }

        DateTime? TransactionTimestamp { get; set; }

        string AssetId { get; set; }

        string Amount { get; set; }

        string Hash { get; set; }

        Int64 BlockCount { get; set; }

        TransactionType TransactionType { get; set; }

        AddressObservationType Type { get; set; }
    }

    public enum TransactionType
    {
        receive,
        send,
    }
}
