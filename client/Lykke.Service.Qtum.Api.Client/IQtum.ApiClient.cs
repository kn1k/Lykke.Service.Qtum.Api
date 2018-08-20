using JetBrains.Annotations;

namespace Lykke.Service.Qtum.Api.Client
{
    /// <summary>
    /// Qtum.Api client interface.
    /// </summary>
    [PublicAPI]
    public interface IQtumApiClient
    {
        /// <summary>Application Api interface</summary>
        IQtumApiApi Api { get; }
    }
}
