using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Qtum.Api.Core.Domain.DirectNodeApi
{
    public interface ITxResult
    {
        string result { get; set; }
        ITxError error { get; set; }
        string id { get; set; }
    }
}
