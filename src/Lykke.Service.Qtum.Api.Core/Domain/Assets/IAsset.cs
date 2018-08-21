namespace Lykke.Service.Qtum.Api.Core.Domain.Assets
{
    /// <summary>
    /// Asset interface
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// Asset ID
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Asset display name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Asset accuracy - maximum number
        /// of significant decimal digits to the right
        /// of the decimal point in the asset amount.
        /// Valid range: [0..28]
        /// </summary>
        int Accuracy { get; }
        
        /// <summary>
        /// Asset address, which identifies
        /// asset in the blockchain, if applicable
        /// for the given blockchain.
        /// Can be empty
        /// </summary>
        string Address { get;  }
        
    }
}
