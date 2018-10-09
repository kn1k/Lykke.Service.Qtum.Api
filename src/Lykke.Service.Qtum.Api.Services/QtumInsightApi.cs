using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs;
using Lykke.Service.Qtum.Api.Core.Domain.InsightApi.Status;
using Lykke.Service.Qtum.Api.Core.Services;
using Lykke.Service.Qtum.Api.Services.InsightApi;
using Lykke.Service.Qtum.Api.Services.InsightApi.AddrTxs;
using Lykke.Service.Qtum.Api.Services.InsightApi.Status;
using Microsoft.AspNetCore.Http;
using NBitcoin;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Lykke.Service.Qtum.Api.Services
{
    public class QtumInsightApi : IInsightApiService
    {
        private readonly string _url;
        private readonly string _directApiUrl;
        private readonly string _directApiUserName;
        private readonly string _directApiPassword;

        public QtumInsightApi(string url, string directApiUrl, string directApiUserName, string directApiPassword)
        {
            _url = url;
            _directApiUrl = directApiUrl;
            _directApiUserName = directApiUserName;
            _directApiPassword = directApiPassword;
        }

        public async Task<IAddrTxs> GetAddrTxsAsync(BitcoinAddress address, int from = 0, int to = 50)
        {
            var client = new RestClient($"{_url}/addrs/{address}/txs?from={from}&to={to}");
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteTaskAsync(request);

            if (response.IsSuccessful)
            {
                return JObject.Parse(response.Content).ToObject<AddrTxs>();
            }
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw new HttpRequestException("Network transport error (network is down, failed DNS lookup, etc)", response.ErrorException);
            }
            else
            {
                throw new ArgumentException(response.Content, response.ErrorException);
            }
        }   

        /// <inheritdoc/>
        public async Task<IStatus> GetStatusAsync()
        {
            var client = new RestClient($"{_url}/status");
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteTaskAsync(request);

            if (response.IsSuccessful)
            {
                return JObject.Parse(response.Content).ToObject<Status>();
            }
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw new HttpRequestException("Network transport error (network is down, failed DNS lookup, etc)", response.ErrorException);
            }
            else
            {
                throw new ArgumentException(response.Content, response.ErrorException);
            }
        }

        /// <inheritdoc/>
        public async Task<List<IUtxo>> GetUtxoAsync(BitcoinAddress address)
        {
            var client = new RestClient($"{_url}/addr/{address}/utxo");
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteTaskAsync<List<Utxo>>(request);

            if (response.IsSuccessful)
            {
                return response.Data.Select(x => (IUtxo) x).ToList();
            }
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw new HttpRequestException("Network transport error (network is down, failed DNS lookup, etc)", response.ErrorException);
            }
            else
            {
                throw new ArgumentException(response.Content, response.ErrorException);
            }
        }

        /// <inheritdoc/>
        public async Task<(ITxId txId, IErrorResponse error)> TxSendAsync(IRawTx rawTx)
        {
            // Due to strange issue with "unconfirmed" transactions broadcasted trough InsightAPI - used direct node to broadcast.

            var client = new RestClient(_directApiUrl);
            var request = new RestRequest(Method.POST);

            var authInfo = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes($"{_directApiUserName}:{_directApiPassword}"));
            request.AddHeader("Authorization", "Basic " + authInfo);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { jsonrpc = "1.0", id = "", method = "sendrawtransaction", @params = new string[] { rawTx.rawtx } });

            var response = await client.ExecuteTaskAsync<TxResult>(request);

            if (response.IsSuccessful)
            {
                if (response.Data.error != null)
                {
                    return (null, new ErrorResponse { error = $"{response.Data.error.message} ({response.Data.error.code})" });
                }

                return (new TxId { txid = response.Data.result }, null);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return (null, new ErrorResponse {error = response.Content});
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

        /// <inheritdoc/>SubstructFees
        public async Task<ITxInfo> GetTxByIdAsync(ITxId txId)
        {
            var client = new RestClient($"{_url}/tx/{txId.txid}");
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteTaskAsync(request);

            if (response.IsSuccessful)
            {
                return JObject.Parse(response.Content).ToObject<TxInfo>();
            }

            if (response.ResponseStatus == ResponseStatus.Error)
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
