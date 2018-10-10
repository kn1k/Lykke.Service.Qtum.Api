using System;
using System.Linq;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.Qtum.Api.Core.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using Newtonsoft.Json;
using Utils = Common.Utils;

namespace Lykke.Service.Qtum.Api.Helpers
{
    public static class ValidateExtenstions
    {
        /// <summary>
        /// Validate Continuation params extenstion
        /// </summary>
        /// <param name="self">Validation information <see cref="ModelStateDictionary"/></param>
        /// <param name="continuation">Continuation info string</param>
        /// <returns>Is continuation valid</returns>
        public static bool IsValidContinuationParameter(this ModelStateDictionary self, string continuation)
        {
            if (string.IsNullOrEmpty(continuation))
            {
                return true;
            }

            try
            {
                JsonConvert.DeserializeObject<TableContinuationToken>(Utils.HexToString(continuation));
                return true;
            }
            catch(Exception exception)
            {
                self.AddModelError(nameof(continuation), exception.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Validate address params extenstion
        /// </summary>
        /// <param name="self">Validation information <see cref="ModelStateDictionary"/></param>
        /// <param name="address">Address</param>
        /// <param name="blockchainService">Blockchain service <see cref="IBlockchainService"/></param>
        /// <returns>Is address valid</returns>
        public static bool IsValidAddressParameter(this ModelStateDictionary self, string address, IBlockchainService blockchainService)
        {
            if (!blockchainService.IsAddressValid(address, out var exception))
            {
                self.AddModelError(nameof(address), exception.Message);

                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Validate take params extenstion
        /// </summary>
        /// <param name="self">Validation information <see cref="ModelStateDictionary"/></param>
        /// <param name="take">Take param</param>
        /// <returns>Is take param valid <see cref="bool"/></returns>
        public static bool IsValidTakeParameter(this ModelStateDictionary self, int take)
        {
            if (take <= 0)
            {
                self.AddModelError(nameof(take), "Must be greater than zero");

                return false;
            }

            return true;
        }

        public static bool IsBuildSingleTransactionRequestValid(this ModelStateDictionary self, BuildSingleTransactionRequest requestParams, IBlockchainService blockchainService)
        {
            if (requestParams.OperationId == null || requestParams.OperationId == System.Guid.Empty)
            {
                return false;
            }

            if (requestParams.ToAddress == null || !blockchainService.IsAddressValid(requestParams.ToAddress))
            {
                return false;
            }

            if (requestParams.FromAddress == null || !blockchainService.IsAddressValid(requestParams.FromAddress))
            {
                return false;
            }

            return true;
        }
        
        public static bool IsValidOperationIdParameter(this ModelStateDictionary self, string operationId)
        {
            if (operationId == null)
            {
                self.AddModelError(nameof(operationId), "Can't be null");

                return false;
            }

            if (!Guid.TryParse(operationId, out var operationIdGuid))
            {
                self.AddModelError(nameof(operationId), "Invalid");

                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Validate broadcasted signed transaction
        /// </summary>
        /// <param name="self">Validation information <see cref="ModelStateDictionary"/></param>
        /// <param name="broadcastTransactionRequest"><see cref="BroadcastTransactionRequest"/></param>
        /// <param name="network"></param>
        /// <returns>Is broadcasted signed transaction valid</returns>
        public static bool IsValid(this ModelStateDictionary self, BroadcastTransactionRequest broadcastTransactionRequest, Network network)
        {
            try
            {
                if (broadcastTransactionRequest.SignedTransaction == null)
                {
                    self.AddModelError(nameof(broadcastTransactionRequest.SignedTransaction), "Can't be null");

                    return false;
                }

                try
                {
                    if (Transaction.Parse(broadcastTransactionRequest.SignedTransaction, network).Equals(null))
                    {
                        self.AddModelError(nameof(broadcastTransactionRequest.SignedTransaction), "Unable to parse passed hex string");
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    self.AddModelError(nameof(broadcastTransactionRequest.SignedTransaction), ex.Message);
                    return false;
                }

                if (broadcastTransactionRequest.OperationId.Equals(Guid.Empty))
                {
                    self.AddModelError(nameof(broadcastTransactionRequest.OperationId), "Can't be Empty");

                    return false;
                }


                return true;
            }
            catch (Exception e)
            {
                self.AddModelError(nameof(broadcastTransactionRequest.OperationId), e.Message);
                return false;
            }
        }

        /// <summary>
        /// Cast Validation information to Lykke error response
        /// </summary>
        /// <param name="self">Validation information <see cref="ModelStateDictionary"/></param>
        /// <param name="summaryMessage">Summary error message</param>
        /// <returns>Lykke error response <see cref="ErrorResponse"/></returns>
        public static ErrorResponse ToErrorResponse(this ModelStateDictionary self, string summaryMessage = null)
        {
            var response = new ErrorResponse();
            
            response.ErrorMessage = summaryMessage;
            
            foreach (var state in self)
            {
                var messages = state.Value.Errors
                    .Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                    .Select(e => e.ErrorMessage)
                    .Concat(state.Value.Errors
                        .Where(e => string.IsNullOrWhiteSpace(e.ErrorMessage))
                        .Select(e => e.Exception.Message))
                    .ToList();

                foreach (var message in messages)
                {
                    response.AddModelError(state.Key, message);
                }
                
            }

            return response;
        }
    }
}
