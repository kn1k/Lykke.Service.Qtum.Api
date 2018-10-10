using Lykke.Service.Qtum.Api.Core.Domain.DirectNodeApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Services.DirectNodeApi
{
    public class TxError : ITxError
    {
        public int? code { get; set; }
        public string message { get; set; }
    }
}
