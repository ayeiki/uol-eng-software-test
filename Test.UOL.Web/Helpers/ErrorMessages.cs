namespace Test.UOL.Web.Helpers;
public static class ErrorMessages
{
    public const string CupomInvalid = "Cupom inválido.";    
    public const string CupomAlreadyApplied = "Já existe um cupom aplicado, remova-o antes de aplicar outro.";
}
public sealed class CupomException : Exception
{
    public CupomException(string message) : base(message) { }
}