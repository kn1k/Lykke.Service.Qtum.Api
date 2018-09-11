namespace Lykke.Service.Qtum.Api.Core.Domain.Transactions
{
    public enum TransactionState
    {
        NotSigned = 1,
        Signed = 2,
        Broadcasted = 3,
        Confirmed = 4,
        Failed = 5,
        BlockChainFailed = 6
    }
}
