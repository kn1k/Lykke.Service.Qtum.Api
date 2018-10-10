using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Services.DirectNodeApi;
using Lykke.Service.Qtum.Api.Services.InsightApi;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.Qtum.Api.Services
{
    public class DirectNodeApiService : IDirectNodeApiService
    {
        private readonly string _url;
        private readonly string _username;
        private readonly string _password;

        public DirectNodeApiService(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        /// <inheritdoc/>
        public async Task<(ITxId txId, IErrorResponse error)> TxSendAsync(IRawTx rawTx)
        {
            var client = new RestClient(_url);
            var request = new RestRequest(Method.POST);

            var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{_username}:{_password}"));
            request.AddHeader("Authorization", "Basic " + authInfo);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { jsonrpc = "1.0", id = "", method = "sendrawtransaction", @params = new string[] { rawTx.rawtx } });

            var response = await client.ExecuteTaskAsync<TxResult>(request);

            if (response.IsSuccessful)
            {
                return (new TxId { txid = response.Data.result }, null);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    return (null, new ErrorResponse { message = response.Data?.error?.message, code = response.Data?.error?.code });
                }
                else if (response.ResponseStatus == ResponseStatus.Error)
                {
                    throw new HttpRequestException("Network transport error (network is down, failed DNS lookup, etc)", response.ErrorException);
                }
                else
                {
                    throw new ArgumentException(response.Content, response.ErrorException);
                }
            }
        }
    }
}
