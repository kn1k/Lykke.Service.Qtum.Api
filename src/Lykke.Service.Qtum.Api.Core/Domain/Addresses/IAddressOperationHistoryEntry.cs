using System;

namespace Lykke.Service.Qtum.Api.Core.Domain.Addresses
{
    public interface IAddressOperationHistoryEntry
    {
        Guid OperationId { get; set; }

        string Hash { get; set; }

        string Address { get; set; }

        AddressObservationType Type { get; set; }

        DateTime TransactionTimestamp { get; set; }
    }
}
