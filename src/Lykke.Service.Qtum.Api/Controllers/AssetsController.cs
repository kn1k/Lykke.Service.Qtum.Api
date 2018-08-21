using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Sdk.Health;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Qtum.Api.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        /// <summary>
        ///  Get batch blockchain assets (coins, tags)
        /// </summary>
        /// <param name="take">Amount of the returned assets should not exceed take</param>
        /// <param name="continuation">Optional continuation contains context of the previous request</param>
        /// <returns>Batch blockchain assets (coins, tags)</returns>
        [HttpGet]
        [SwaggerOperation("GetAssets")]
        [ProducesResponseType(typeof(PaginationResponse<AssetContract>), (int)HttpStatusCode.OK)]
        public IActionResult GetAssets([Required, FromQuery]int take, [FromQuery]string continuation = null)
        {
            if (ModelState.IsValidTakeParameter(take))
            {
                return StatusCode((int)HttpStatusCode.OK, 
                    PaginationResponse.From(
                        null,
                        _assetService.GetAssets(take, continuation).Select(x => new AssetContract
                        {
                            AssetId = x.Id,
                            Name = x.Name,
                            Accuracy = x.Accuracy
                        }).ToList()
                    ));
            } 
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ModelState.ToErrorResponse("Invalid params"));
            }
        }

        /// <summary>
        /// Get specified asset (coin, tag)
        /// </summary>
        /// <param name="assetId">Asset id</param>
        /// <returns>Specified asset (coin, tag)</returns>
        [HttpGet("{assetId}")]
        [SwaggerOperation("GetAsset")]
        [ProducesResponseType(typeof(AssetContract), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public IActionResult GetAsset(string assetId)
        {
            var asset = _assetService.GetAsset(assetId);
            if (asset != null)
            {
                return Ok(new AssetContract
                {
                    AssetId = asset.Id,
                    Name = asset.Name,
                    Accuracy = asset.Accuracy
                });
            }
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
