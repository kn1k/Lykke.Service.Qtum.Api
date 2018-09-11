using Newtonsoft.Json;

namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IVout 
    {
        string Value { get; set; }

        long N { get; set; }

        IScriptPubKey ScriptPubKey { get; set; }

        object SpentTxId { get; set; }

        object SpentIndex { get; set; }

        object SpentHeight { get; set; }
    }
}
