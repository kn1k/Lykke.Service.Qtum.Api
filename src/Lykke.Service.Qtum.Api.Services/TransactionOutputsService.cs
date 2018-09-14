
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Repositories.TransactionOutputs;
using Lykke.Service.Qtum.Api.Core.Repositories.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Services
{
    public class TransactionOutputsService : ITransactionOutputsService
    {
        private readonly IBlockchainService _blockchainService;
        private readonly ISpentOutputRepository _spentOutputRepository;

        public TransactionOutputsService(IBlockchainService blockchainService, ISpentOutputRepository spentOutputRepository)
        {
            _blockchainService = blockchainService;
            _spentOutputRepository = spentOutputRepository;
        }

        public async Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int confirmationsCount = 0)
        {
            return await Filter(await _blockchainService.GetUnspentOutputsAsync(address, confirmationsCount));
        }

        private async Task<IEnumerable<Coin>> Filter(IList<Coin> coins)
        {
            var spentOutputs = new HashSet<OutPoint>((await _spentOutputRepository.GetSpentOutputs(coins.Select(o => new Output(o.Outpoint))))
                                                                                  .Select(o => new OutPoint(uint256.Parse(o.TransactionHash), o.N)));
            return coins.Where(c => !spentOutputs.Contains(c.Outpoint));
        }
    }
}
