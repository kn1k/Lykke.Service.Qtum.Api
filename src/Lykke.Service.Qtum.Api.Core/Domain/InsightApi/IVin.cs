namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IVin
    {
        string Txid { get; set; }

        long Vout { get; set; }

        long Sequence { get; set; }

        long N { get; set; }

        IScriptSig ScriptSig { get; set; }

        string Addr { get; set; }

        long ValueSat { get; set; }

        double Value { get; set; }

        object DoubleSpentTxId { get; set; }
    }
}
