using System.Collections.Generic;
using System.Linq;
using Lykke.Service.Qtum.Api.Core.Domain.Assets;
using Lykke.Service.Qtum.Api.Core.Services;

namespace Lykke.Service.Qtum.Api.Services
{
    /// <summary>
    /// Asset setvice
    /// </summary>
    public class AssetService : IAssetService
    {
        private readonly List<IAsset> _assets = new List<IAsset> { new Core.Domain.Assets.Qtum() };
        
        /// <inheritdoc/>
        public List<IAsset> GetAssets(int take, string continuation = null)
        {
            return _assets;
        }

        /// <inheritdoc/>
        public IAsset GetAsset(string id)
        {
            return _assets.FirstOrDefault(x => x.Id.Equals(id));
        }
        
        /// <inheritdoc/>
        public IAsset GetQtumAsset()
        {
            return _assets.FirstOrDefault();
        }
    }
}
