using Lykke.Service.Qtum.Api.Core.Domain.TransactionOutputs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Repositories.TransactionOutputs
{
    public interface ISpentOutputRepository
    {
        Task InsertSpentOutputs(Guid transactionId, IEnumerable<IOutput> outputs);

        Task<IEnumerable<IOutput>> GetSpentOutputs(IEnumerable<IOutput> outputs);

        Task RemoveOldOutputs(DateTime bound);
    }
}
