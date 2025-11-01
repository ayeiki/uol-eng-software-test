namespace Test.UOL.Web.Entities;

public class CupomItem
{
    public string? key { get; set; }
    public string? value { get; set; }
    public string? type { get; set; }
}

public enum CupomType { Percentage, Fixed } // Tipos suportados de cupom.