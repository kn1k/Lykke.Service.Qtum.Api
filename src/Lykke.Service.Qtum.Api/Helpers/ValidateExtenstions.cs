using System.Linq;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Service.Qtum.Api.Helpers
{
    public static class ValidateExtenstions
    {
        /// <summary>
        /// Validate take params extenstions
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
