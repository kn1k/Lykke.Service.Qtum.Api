using System.Net;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.BlockchainApi.Contract.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Qtum.Api.Controllers
{
[Route("api/[controller]")]
    public class ConstantsController : Controller
    {        
        /// <summary>
        /// Get blockchain integration constants.
        /// </summary>
        /// <returns>Blockchain integration constants <see cref="ConstantsResponse"/>.</returns>
        [HttpGet]
        [SwaggerOperation("Constants")]
        [ProducesResponseType(typeof(ConstantsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
        public IActionResult GetConstants()
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }
    }
}
