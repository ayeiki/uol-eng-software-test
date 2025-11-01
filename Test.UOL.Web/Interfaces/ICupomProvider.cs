using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;

public interface ICupomProvider
{
    /// <summary>
    /// Busca o cupom do arquivo json pelo código (ou null se não existir)
    /// </summary>
    /// <param name="key">Código do cupom</param>
    CupomItem? GetCupom(string key);
}