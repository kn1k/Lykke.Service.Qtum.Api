namespace Lykke.Service.Qtum.Api.Core.Domain.Addresses
{
    public interface IAddressObservation
    {
        string Address { get; set; }

        AddressObservationType Type { get; set; }
    }
}
