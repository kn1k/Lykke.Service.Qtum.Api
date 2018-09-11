using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Addresses;
using Lykke.Service.Qtum.Api.Core.Domain.Addresses;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Controllers
{
    [Route("api/transactions/[controller]")]
    public class HistoryController : Controller
    {
        private readonly IHistoryService<AddressHistoryEntry, AddressObservation> _historyService;

        private readonly IAssetService _assetService;
        private readonly IBlockchainService _blockchainService;
        private readonly ILog _log;

        public HistoryController(
            IHistoryService<AddressHistoryEntry, AddressObservation> historyService,
            IAssetService assetService, IBlockchainService blockchainService, ILogFactory log)
        {
            _historyService = historyService;
            _assetService = assetService;
            _blockchainService = blockchainService;
            _log = log.CreateLog(this);
        }

        /// <summary>
        /// Start observation of the transactions that transfer fund from the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("from/{address}/observation")]
        [SwaggerOperation("AddHistoryObservationFrom")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddHistoryObservationFromAsync(string address)
        {
            if (ModelState.IsValidAddressParameter(address, _blockchainService))
            {
                var addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.From
                };
                if (!await _historyService.IsAddressObservedAsync(addressObservation) && await _historyService.AddAddressObservationAsync(addressObservation))
                {
                    _log.Info(nameof(AddHistoryObservationFromAsync), $"Start observing history from {address}", JObject.FromObject(addressObservation).ToString());
                    
                    return Ok();
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Conflict, ModelState.ToErrorResponse("Transactions from the address are already observed"));
                }
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ModelState.ToErrorResponse("Invalid address"));
            }
        }

        /// <summary>
        /// Start observation of the transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("to/{address}/observation")]
        [SwaggerOperation("AddHistoryObservationFrom")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddHistoryObservationToAsync(string address)
        {
            if (ModelState.IsValidAddressParameter(address, _blockchainService))
            {
                var addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.To
                };
                if (!await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.AddAddressObservationAsync(addressObservation))
                {
                    _log.Info(nameof(AddHistoryObservationToAsync), $"Start observing history to {address}", JObject.FromObject(addressObservation).ToString());

                    return Ok();
                }

                return StatusCode((int)HttpStatusCode.Conflict,
                    ErrorResponse.Create("Transactions to the address are already observed"));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }

        /// <summary>
        /// Get completed transactions that transfer fund from the address 
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <param name="take">Amount of the returned transactions should not exceed take</param>
        /// <param name="afterHash">Transaction hash</param>
        /// <returns>Historical transaction contract</returns>
        [HttpGet("from/{address}")]
        [SwaggerOperation("GetHistoryFrom")]
        [ProducesResponseType(typeof(IEnumerable<HistoricalTransactionContract>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<HistoricalTransactionContract>> GetHistoryFromAsync(string address,
            [FromQuery] int take = 100, [FromQuery] string afterHash = null)
        {
            var history = await _historyService.GetAddressHistoryAsync(take,
                Enum.GetName(typeof(AddressObservationType), AddressObservationType.From), address, afterHash);

            return history.items?.OrderByDescending(x => x.BlockCount).Select(x => new HistoricalTransactionContract
            {
                Amount = x.Amount, // TODO (nkataev) need to convert?
                AssetId = _assetService.GetQtumAsset().Id,
                FromAddress = x.FromAddress,
                ToAddress = x.ToAddress,
                Hash = x.Hash,
                Timestamp = x.TransactionTimestamp.Value,
                TransactionType = x.TransactionType == Core.Services.TransactionType.send
                    ? BlockchainApi.Contract.Transactions.TransactionType.Send
                    : BlockchainApi.Contract.Transactions.TransactionType.Receive
            });
        }

        /// <summary>
        /// Get completed transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <param name="take">Amount of the returned transactions should not exceed take</param>
        /// <param name="afterHash">Transaction hash</param>
        /// <returns>Historical transaction contract</returns>
        [HttpGet("to/{address}")]
        [SwaggerOperation("GetHistoryTo")]
        [ProducesResponseType(typeof(IEnumerable<HistoricalTransactionContract>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<HistoricalTransactionContract>> GetHistoryToAsync(string address, [FromQuery] int take,
            [FromQuery] string afterHash = null)
        {
            var history = await _historyService.GetAddressHistoryAsync(take,
                Enum.GetName(typeof(AddressObservationType), AddressObservationType.To), address, afterHash);
            return history.items?.OrderByDescending(x => x.BlockCount).Select(
                x => new HistoricalTransactionContract
                {
                    Amount = x.Amount,
                    AssetId = _assetService.GetQtumAsset().Id,
                    FromAddress = x.FromAddress,
                    ToAddress = x.ToAddress,
                    Hash = x.Hash,
                    Timestamp = x.TransactionTimestamp.Value,
                    TransactionType = x.TransactionType == Core.Services.TransactionType.send
                        ? BlockchainApi.Contract.Transactions.TransactionType.Send
                        : BlockchainApi.Contract.Transactions.TransactionType.Receive
                });
        }

        /// <summary>
        /// Stop observation of the transactions that transfer fund from the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Status code</returns>
        [HttpDelete("from/{address}/observation")]
        [HttpDelete("from/")]
        [SwaggerOperation("DeleteHistoryFrom")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteHistoryFromAsync(string address)
        {
            if (ModelState.IsValidAddressParameter(address, _blockchainService))
            {
                var addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.From
                };
                if (await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.RemoveAddressObservationAsync(addressObservation))
                {
                    _log.Info(nameof(AddHistoryObservationToAsync),
                        $"Stop observing history from {address}", JObject.FromObject(addressObservation).ToString());

                    return Ok();
                }

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }

        /// <summary>
        /// Stop observation of the transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Status code</returns>
        [HttpDelete("to/{address}/observation")]
        [HttpDelete("to/")]
        [SwaggerOperation("DeleteHistoryTo")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteHistoryToAsync(string address)
        {
            if (ModelState.IsValidAddressParameter(address, _blockchainService))
            {
                var addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.To
                };
                if (await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.RemoveAddressObservationAsync(addressObservation))
                {
                    _log.Info(nameof(AddHistoryObservationToAsync),
                        $"Stop observe history to {address}", JObject.FromObject(addressObservation).ToString());

                    return Ok();
                }

                return StatusCode((int)HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }
    }
}
