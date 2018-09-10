using JetBrains.Annotations;

namespace Lykke.Service.QtumApi.Client
{
    /// <summary>
    /// QtumApi client interface.
    /// </summary>
    [PublicAPI]
    public interface IQtumApiClient
    {
        /// <summary>Application Api interface</summary>
        IQtumApiApi Api { get; }
    }
}
