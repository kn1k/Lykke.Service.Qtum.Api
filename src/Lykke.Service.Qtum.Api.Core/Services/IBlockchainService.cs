using NBitcoin;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    /// <summary>
    /// Blockchain service interface
    /// </summary>
    public interface IBlockchainService
    {
        /// <summary>
        /// Get network
        /// </summary>
        /// <returns>Blockchain network <see cref="Network"/></returns>
        Network GetNetwork();
        
        /// <summary>
        /// Address validate
        /// </summary>
        /// <param name="address">Blockchain address</param>
        /// <returns>Is assress valid</returns>
        bool IsAddressValid(string address);
    }
}
