using System;

namespace Lykke.Service.Qtum.Api.Core.Domain.Transactions
{
    public interface ITransactionBody
    {
        Guid OperationId { get; set; }

        string UnsignedTransaction { get; set; }

        string SignedTransaction { get; set; }
    }
}
