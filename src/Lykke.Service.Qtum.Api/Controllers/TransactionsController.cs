using System;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.TransactionOutputs;
using Lykke.Service.Qtum.Api.AzureRepositories.Entities.Transactions;
using Lykke.Service.Qtum.Api.Core.Domain.Transactions;
using Lykke.Service.Qtum.Api.Core.Exceptions;
using Lykke.Service.Qtum.Api.Core.Helpers;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Qtum.Api.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly ILog _log;
        private readonly CoinConverter _coinConverter;
        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation, SpentOutputEntity> _transactionService;
        private readonly IBlockchainService _blockchainService;

        public TransactionsController(ILogFactory logFactory, 
            IBlockchainService blockchainService, 
            ITransactionService<TransactionBody, TransactionMeta, TransactionObservation, SpentOutputEntity> transactionService,
            CoinConverter coinConverter)
        {
            _blockchainService = blockchainService;
            _transactionService = transactionService;
            _coinConverter = coinConverter;
            _log = logFactory.CreateLog(this);
        }

        #region build

        /// <summary>
        /// Build not signed transaction to transfer from the single source to the single destination
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("single")]
        [SwaggerOperation("BuildNotSignedSendTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.Conflict)]
        public async Task<IActionResult> BuildNotSignedSingleSendTransactionAsync(
            [FromBody] BuildSingleTransactionRequest buildTransactionRequest)
        {
            if (ModelState.IsBuildSingleTransactionRequestValid(buildTransactionRequest, _blockchainService))
            {
                if (await _transactionService.IsTransactionAlreadyBroadcastAsync(buildTransactionRequest.OperationId))
                {
                    return StatusCode((int)HttpStatusCode.Conflict,
                        ErrorResponse.Create(
                            "Transaction is already broadcasted or [DELETE] /api/transactions/broadcast/{operationId} is called"));
                }

                var balance = await _blockchainService.GetAddressBalanceAsync(_blockchainService.ParseAddress(buildTransactionRequest.FromAddress));

                if (balance < BigInteger.Parse(buildTransactionRequest.Amount))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest,
                        BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.NotEnoughBalance));
                }

                try
                {
                    var unsignTransaction = await _transactionService.GetUnsignSendTransactionAsync(
                        buildTransactionRequest.OperationId,
                        buildTransactionRequest.FromAddress,
                        buildTransactionRequest.ToAddress,
                        buildTransactionRequest.Amount,
                        buildTransactionRequest.AssetId,
                        buildTransactionRequest.IncludeFee);

                    return StatusCode((int) HttpStatusCode.OK, new BuildTransactionResponse
                    {
                        TransactionContext = unsignTransaction
                    });
                }
                catch (NBitcoin.NotEnoughFundsException e)
                {
                    _log.Error(e);
                    return StatusCode((int) HttpStatusCode.BadRequest,
                        BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.NotEnoughBalance));
                }
                catch (AmountIsTooSmallException e)
                {
                    _log.Error(e);
                    return StatusCode((int) HttpStatusCode.BadRequest,
                        BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.AmountIsTooSmall));
                }
                
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
        }

        /// <summary>
        /// Build not signed transaction with many inputs
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("many-inputs")]
        [SwaggerOperation("BuildNotSignedManyInputsTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult BuildNotSignedManyInputsTransaction(
            [FromBody] BuildTransactionWithManyInputsRequest buildTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Build not signed transaction with many outputs
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("many-outputs")]
        [SwaggerOperation("BuildNotSignedManyOutputsTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult BuildNotSignedManyOutputsTransaction(
            [FromBody] BuildTransactionWithManyOutputsRequest buildTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        #endregion

        #region broadcast

        /// <summary>
        /// Broadcast the signed transaction
        /// </summary>
        /// <param name="broadcastTransactionRequest">Broadcast transaction request</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpPost("broadcast")]
        [SwaggerOperation("BroadcastSignedTransaction")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BroadcastSignedTransactionAsync(
            [FromBody] BroadcastTransactionRequest broadcastTransactionRequest)
        {
            if (!ModelState.IsValid(broadcastTransactionRequest, _blockchainService.GetNetwork()))
            {
                return BadRequest(ModelState.ToErrorResponse("Transaction invalid"));
            }

            var result = await _transactionService.BroadcastSignedTransactionAsync(
                broadcastTransactionRequest.OperationId, broadcastTransactionRequest.SignedTransaction);

            if (!result)
            {
                return StatusCode((int) HttpStatusCode.Conflict,
                    ErrorResponse.Create(
                        "Transaction with specified operationId and signedTransaction has already been broadcasted"));
            }

            var txMeta = await _transactionService.UpdateTransactionBroadcastStatusAsync(broadcastTransactionRequest.OperationId);

            if (txMeta.State == TransactionState.Failed)
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("buildingShouldBeRepeated"));
            }

            _log.Info(nameof(BroadcastSignedTransactionAsync),
                JObject.FromObject(broadcastTransactionRequest).ToString(),
                $"Transaction broadcasted {broadcastTransactionRequest.OperationId}");

            return Ok();
        }

        /// <summary>
        /// Get broadcasted transaction with single input and output
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/single/{operationId}")]
        [SwaggerOperation("GetBroadcastedSingleTransaction")]
        [ProducesResponseType(typeof(BroadcastedSingleTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetBroadcastedSingleTransactionAsync(string operationId)
        {
            if (ModelState.IsValidOperationIdParameter(operationId))
            {               
                var txMeta = await _transactionService.GetTransactionMetaAsync(operationId);
                if (txMeta != null)
                {
                    txMeta = await _transactionService.UpdateTransactionBroadcastStatusAsync(new Guid(operationId), true);
                    
                    BroadcastedTransactionState state;
                    BlockchainErrorCode? blockchainErrorCode = null;

                    switch (txMeta.State)
                    {
                        case TransactionState.Confirmed:
                            state = BroadcastedTransactionState.Completed;
                            break;
                        case TransactionState.Failed:
                        case TransactionState.BlockChainFailed:
                            state = BroadcastedTransactionState.Failed;
                            
                            //TODO: support different codes from node api
                            blockchainErrorCode = BlockchainErrorCode.BuildingShouldBeRepeated;
                            
                            break;
                        default:
                            state = BroadcastedTransactionState.InProgress;
                            break;
                    }

                    return Ok(new BroadcastedSingleTransactionResponse
                    {
                        OperationId = txMeta.OperationId,
                        State = state,
                        Timestamp = (txMeta.CompleteTimestamp ?? txMeta.BroadcastTimestamp).Value,
                        Amount = txMeta.Amount,
                        Fee = txMeta.Fee,
                        Hash = txMeta.TxId,
                        Error = txMeta.Error,
                        ErrorCode = blockchainErrorCode,
                        Block = txMeta.BlockCount
                    });
                }
                else
                    return StatusCode((int) HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ModelState.ToErrorResponse());
            }
        }

        /// <summary>
        /// Remove specified transaction from the broadcasted transactions
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>HttpStatusCode</returns>
        [HttpDelete("broadcast/{operationId}")]
        [SwaggerOperation("DeleteBroadcastedTransaction")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBroadcastedTransactionAsync(string operationId = null)
        {
            if (ModelState.IsValidOperationIdParameter(operationId))
            {
                TransactionObservation transactionObservation = new TransactionObservation
                {
                    OperationId = new Guid(operationId)
                };
                if (await _transactionService.IsTransactionObservedAsync(transactionObservation) &&
                    await _transactionService.RemoveTransactionObservationAsync(transactionObservation))
                {
                    _log.Info(nameof(DeleteBroadcastedTransactionAsync),
                        JObject.FromObject(transactionObservation).ToString(), $"Stop observe operation {operationId}");
                    return Ok();
                }

                return StatusCode((int) HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ModelState.ToErrorResponse());
            }
        }

        #endregion


        #region NotImplemented

                /// <summary>
        /// Get broadcasted transaction with with many inputs
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/many-inputs/{operationId}")]
        [SwaggerOperation("GetBroadcastedManyInputsTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionWithManyInputsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult GetBroadcastedManyInputsTransaction(string operationId)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }


        /// <summary>
        /// Get broadcasted transaction with with many outputs
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/many-outputs/{operationId}")]
        [SwaggerOperation("GetBroadcastedManyOutputsTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionWithManyOutputsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult GetBroadcastedManyOutputsTransaction(string operationId)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }
        
        /// <summary>
        ///  Rebuild not signed transaction with the specified fee factor
        /// </summary>
        /// <param name="rebuildTransactionRequest">Rebuild transaction request</param>
        /// <returns>Rebuild transaction response</returns>
        [HttpPut]
        [SwaggerOperation("RebuildNotSignedTransaction")]
        [ProducesResponseType(typeof(RebuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotAcceptable)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult RebuildNotSignedTransaction([FromBody] RebuildTransactionRequest rebuildTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }
        
        /// <summary>
        /// Build not signed receive transaction to transfer from the single source to the single destination
        /// </summary>
        /// <param name="buildSingleReceiveTransactionRequest">Build transaction request <see cref="BuildSingleReceiveTransactionRequest"/></param>
        /// <returns>Build transaction response <see cref="BuildTransactionResponse"/></returns>
        [HttpPost("single/receive")]
        [SwaggerOperation("BuildNotSignedReceiveTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.NotImplemented)]
        public async Task<IActionResult> BuildNotSignedSingleReceiveTransactionAsync(
            [FromBody] BuildSingleReceiveTransactionRequest buildSingleReceiveTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        #endregion
    }
}
