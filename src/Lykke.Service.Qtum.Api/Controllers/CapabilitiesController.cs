using System.Net;
using Common.Log;
using Lykke.Common.Log;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Lykke.Service.BlockchainApi.Contract.Common;

namespace Lykke.Service.Qtum.Api.Controllers
{
    [Route("api/[controller]")]
    public class CapabilitiesController : Controller
    {        
        /// <summary>
        /// Get API capabilities set.
        /// </summary>
        /// <returns>API capabilities set <see cref="CapabilitiesResponse"/>.</returns>
        [HttpGet]
        [SwaggerOperation("Capabilities")]
        [ProducesResponseType(typeof(CapabilitiesResponse), (int)HttpStatusCode.OK)]
        public CapabilitiesResponse GetCapabilities()
        {
            return new CapabilitiesResponse
            {
                IsTransactionsRebuildingSupported = false,
                AreManyInputsSupported = false,
                AreManyOutputsSupported = false,
                CanReturnExplorerUrl = true,
                IsPublicAddressExtensionRequired = false,
                IsReceiveTransactionRequired = false,
                IsTestingTransfersSupported = false
                
            };
        }
    }
}
