using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Core.Repositories.Addresses
{
    public interface IAddressObservationRepository<TAddressObservation> : IRepository<TAddressObservation>
        where TAddressObservation : IAddressObservation
    {

    }
}
