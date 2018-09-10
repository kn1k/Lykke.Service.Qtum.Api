using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Balances;
using Lykke.Service.Qtum.Api.Core.Helpers;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Qtum.Api.Controllers
{
    [Route("api/[controller]")]
    public class BalancesController : Controller
    {
   
        private readonly IBlockchainService _blockchainService;
        private readonly ILog _log;
        private readonly IBalanceService<BalanceObservation, AddressBalance> _balanceService;
        private readonly IAssetService _assetService;
        private readonly CoinConverter _coinConverter;

        public BalancesController(IBlockchainService blockchainService, ILogFactory logFactory, IBalanceService<BalanceObservation, AddressBalance> balanceService, IAssetService assetService, CoinConverter coinConverter)
        {
            _blockchainService = blockchainService;
            _log = logFactory.CreateLog(this);
            _balanceService = balanceService;
            _assetService = assetService;
            _coinConverter = coinConverter;
        }

        /// <summary>
        /// Remember the wallet address to observe
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("{address}/observation")]
        [SwaggerOperation("AddBalanceObservation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddBalanceObservationAsync(string address)
        {
            if (ModelState.IsValidAddressParameter(address, _blockchainService))
            {
                BalanceObservation balanceObservation = new BalanceObservation
                {
                    Address = address,
                };
                if (!await _balanceService.IsBalanceObservedAsync(balanceObservation) && await _balanceService.StartBalanceObservationAsync(balanceObservation))
                {
                    await _log.WriteInfoAsync(nameof(AddBalanceObservationAsync), JObject.FromObject(balanceObservation).ToString(), $"Start observe balance for {address}");
                    return Ok();
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Conflict, ModelState.ToErrorResponse("Specified address is already observed"));
                }
            } else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ModelState.ToErrorResponse("Invalid address"));
            }

        }

        /// <summary>
        /// Forget the wallet address and stop observing its balance
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpDelete("{address}/observation")]
        [SwaggerOperation("RemoveBalanceObservation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveBalanceObservationAsync(string address)
        {
            if (ModelState.IsValidAddressParameter(address, _blockchainService))
            {
                BalanceObservation balanceObservation = new BalanceObservation
                {
                    Address = address
                };
                if (await _balanceService.IsBalanceObservedAsync(balanceObservation) && await _balanceService.StopBalanceObservationAsync(balanceObservation))
                {
                    await _balanceService.RemoveBalanceAsync(new AddressBalance
                    {
                        Address = address
                    });

                    await _log.WriteInfoAsync(nameof(AddBalanceObservationAsync), JObject.FromObject(balanceObservation).ToString(), $"Stop observe balance for {address}");
                    return Ok();
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest,  ModelState.ToErrorResponse("Invalid address"));
            }              
        }

        /// <summary>
        /// Get balances of the observed wallets with non zero balances
        /// </summary>
        /// <param name="take">Amount of the returned wallets should not exceed take</param>
        /// <param name="continuation">Optional continuation contains context of the previous request</param>
        /// <returns>Balances of the observed wallets with non zero balances</returns>
        [HttpGet]
        [SwaggerOperation("GetBalances")]
        [ProducesResponseType(typeof(PaginationResponse<WalletBalanceContract>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBalancesAsync([FromQuery]int take, [FromQuery]string continuation = null)
        {
            if (ModelState.IsValidTakeParameter(take) && ModelState.IsValidContinuationParameter(continuation))
            {
                var balances = await _balanceService.GetBalancesAsync(take, continuation);
                return StatusCode((int)HttpStatusCode.OK, PaginationResponse.From(
                    balances.continuation,
                    balances.items.Select(b => new WalletBalanceContract
                    {
                        Address = b.Address,
                        Balance = _coinConverter.QtumToLykkeQtum(b.Balance),
                        AssetId = _assetService.GetQtumAsset().Id,
                        Block = b.Block
                    }).ToArray()));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
        }
    }
}
