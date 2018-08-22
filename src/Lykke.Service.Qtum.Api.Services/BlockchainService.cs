using System;
using Lykke.Service.Qtum.Api.Core.Services;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Lykke.Service.Qtum.Api.Services
{
    /// <summary>
    /// Blockchain service
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        private readonly Network _network;

        public BlockchainService(Network network)
        {
            _network = network;
        }

        /// <inheritdoc/>
        public Network GetNetwork()
        {
            return _network;
        }
        
        /// <inheritdoc/>
        public bool IsAddressValid(string address, out Exception exception)
        {
            try
            {
                exception = null;
                return BitcoinAddressCreate(address) != null;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }
        
        /// <inheritdoc/>
        public bool IsAddressValid(string address)
        {
            try
            {
                return BitcoinAddressCreate(address) != null;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private BitcoinAddress BitcoinAddressCreate(string address)
        {
            return BitcoinAddress.Create(address, _network);
        }
    }
}
