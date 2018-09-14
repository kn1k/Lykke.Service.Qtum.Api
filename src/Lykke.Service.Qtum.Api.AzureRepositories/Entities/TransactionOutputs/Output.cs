using Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs
{
    public class Output : IOutput
    {
        public Output(OutPoint outpoint)
        {
            TransactionHash = outpoint.Hash.ToString();
            N = (int)outpoint.N;
        }

        public string TransactionHash { get; }
        public int N { get; }
    }
}
