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

        public QtumInsightApi(string url)
        {
            _url = url;
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
            var client = new RestClient($"{_url}/tx/send");
            var request = new RestRequest(Method.POST);

            var jRawTx = JObject.FromObject(rawTx);

            request.AddParameter("application/json; charset=utf-8", jRawTx.ToString(), ParameterType.RequestBody);

            var response = await client.ExecuteTaskAsync<TxId>(request);

            if (response.IsSuccessful)
            {
                return (response.Data, null);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return (null, new ErrorResponse {message = response.Content});
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
