namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IScriptPubKey
    {
        string Hex { get; set; }

        string Asm { get; set; }
    }
    
    public enum TypeEnum { Pubkeyhash };
}
