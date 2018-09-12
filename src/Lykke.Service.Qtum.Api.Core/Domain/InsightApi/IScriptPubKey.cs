namespace Lykke.Service.Qtum.Api.Core.Domain.InsightApi
{
    public interface IScriptPubKey
    {
        string Hex { get; set; }

        string Asm { get; set; }

        string[] Addresses { get; set; }

        TypeEnum? Type { get; set; }
    }
    
    public enum TypeEnum { Pubkeyhash };
}
