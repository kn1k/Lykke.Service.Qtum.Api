namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi.AddrTxs
{
    public interface IVin
    {
        string Txid { get; set; }
        int Vout { get; set; }
        long Sequence { get; set; }
        int N { get; set; }
        IScriptsig ScriptSig { get; set; }
        string Addr { get; set; }
        long ValueSat { get; set; }
        float Value { get; set; }
        string DoubleSpentTxID { get; set; }
    }
}
