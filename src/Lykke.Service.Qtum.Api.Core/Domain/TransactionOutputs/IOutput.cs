using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs
{
    public interface IOutput
    {
        string TransactionHash { get; }

        int N { get; }
    }
}
