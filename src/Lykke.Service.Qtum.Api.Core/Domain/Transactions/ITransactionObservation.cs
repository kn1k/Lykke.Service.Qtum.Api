using System;

namespace Lykke.Service.Qtum.Api.Core.Domain.Transactions
{
    public interface ITransactionObservation
    {
        Guid OperationId { get; set; }
    }
}
