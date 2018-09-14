namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IScriptSig
    {
        string Hex { get; set; }

        string Asm { get; set; }
    }
}
