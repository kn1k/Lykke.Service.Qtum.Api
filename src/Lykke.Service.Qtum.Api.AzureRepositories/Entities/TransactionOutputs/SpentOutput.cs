using Lykke.AzureStorage.Tables;
using Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs
{
    public class SpentOutputEntity : AzureTableEntity, IOutput
    {

        public string TransactionHash { get; set; }
        public int N { get; set; }
        public Guid OperationId { get; set; }

        public static SpentOutputEntity Create(string transactionHash, int n, Guid operationId)
        {
            return new SpentOutputEntity
            {
                PartitionKey = GeneratePartitionKey(transactionHash),
                RowKey = GenerateRowKey(n),
                OperationId = operationId,
                TransactionHash = transactionHash,
                N = n
            };
        }

        public static string GenerateRowKey(int n)
        {
            return n.ToString();
        }

        public static string GeneratePartitionKey(string transactionHash)
        {
            return transactionHash;
        }
    }
}
