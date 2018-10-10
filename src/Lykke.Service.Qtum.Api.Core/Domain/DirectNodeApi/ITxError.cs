using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.DirectNodeApi
{
    public interface ITxError
    {
        int? code { get; set; }
        string message { get; set; }
    }
}
