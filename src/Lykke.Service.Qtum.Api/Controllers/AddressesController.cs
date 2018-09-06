using System.Collections.Generic;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NBitcoin.Qtum;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Qtum.Api.Controllers
{
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        private readonly IBlockchainService _blockchainService;

        public AddressesController(IBlockchainService blockchainService)
        {
            _blockchainService = blockchainService;
        }

        /// <summary>
        /// Check wallet address validity
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>Address validity</returns>
        [HttpGet("{address}/validity")]
        [SwaggerOperation("AddressValidity")]
        [ProducesResponseType(typeof(AddressValidationResponse), (int) HttpStatusCode.OK)]
        public AddressValidationResponse AddressValidityAsync(string address)
        {
            return new AddressValidationResponse
            {
                IsValid = _blockchainService.IsAddressValid(address)
            };
        }

        /// <summary>
        /// Get blockchain explorer URLs.
        /// </summary>
        /// <param name="address">Wallet address/</param>
        /// <returns>Blockchain explorer URLs</returns>
        [HttpGet("{address}/explorer-url")]
        [SwaggerOperation("ExplorerUrl")]
        [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotImplemented)]
        public IActionResult AddressExplorerUrl(string address)
        {
            if (_blockchainService.IsAddressValid(address))
            {
                if (_blockchainService.GetNetwork() == QtumNetworks.Testnet)
                {
                    return StatusCode((int) HttpStatusCode.OK,
                        new List<string> {$"https://testnet.qtum.org/address/{address}"});
                }
                else
                {
                    return StatusCode((int) HttpStatusCode.OK,
                        new List<string> {$"https://qtum.info/address/{address}"});
                }
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }
    }
}
