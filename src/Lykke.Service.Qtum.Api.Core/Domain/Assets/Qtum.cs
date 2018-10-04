namespace Lykke.Service.Qtum.Api.Core.Domain.Assets
{
    /// <summary>
    /// Qtum asset
    /// </summary>
    public class Qtum : IAsset
    {
        /// <inheritdoc/>
        public string Id
        {
            get => "QTUM";
        }

        /// <inheritdoc/>
        public string Name
        {
            get => "QTUM";
        }

        /// <inheritdoc/>
        public int Accuracy
        {
            get => 8;
        }

        /// <inheritdoc/>
        public string Address { get; }
    }
}
