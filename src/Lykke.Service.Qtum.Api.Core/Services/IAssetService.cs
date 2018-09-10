using System.Collections.Generic;
using Lykke.Service.Qtum.Api.Core.Domain.Assets;

namespace Lykke.Service.Qtum.Api.Core.Services
{
    /// <summary>
    /// Assets setrvice interface
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Get list of assets
        /// </summary>
        /// <param name="take">Amount of the returned assets should not exceed take</param>
        /// <param name="continuation">Optional continuation contains context of the previous request</param>
        /// <returns>Batch blockchain assets (coins, tags) <see cref="List{IAsset}"/></returns>
        List<IAsset> GetAssets(int take, string continuation = null);
        
        /// <summary>
        /// Get asset by id
        /// </summary>
        /// <param name="id">Asset id</param>
        /// <returns>Asset <see cref="IAsset"/></returns>
        IAsset GetAsset(string id);
        
        /// <summary>
        /// Get Qtum asset
        /// </summary>
        /// <returns>Asset <see cref="IAsset"/></returns>
        IAsset GetQtumAsset();

    }
}
