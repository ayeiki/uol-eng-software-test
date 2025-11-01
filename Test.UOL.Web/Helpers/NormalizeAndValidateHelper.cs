// Helpers/CouponHelper.cs
using System.Globalization;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Helpers;

public static class NormalizeAndValidateHelper
{
    /// <summary>
    /// Normaliza e valida um CupomItem.
    /// Lança CupomException se inválido.
    /// </summary>
    public static (CupomType Type, decimal Value) NormalizeAndValidate(CupomItem cupom)
    {
        if (cupom is null) throw new CupomException(ErrorMessages.CupomInvalid);

        // Tipo
        var type = (cupom.type ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "percentage" => CupomType.Percentage,
            "fixed"      => CupomType.Fixed,
            _            => throw new CupomException(ErrorMessages.CupomInvalid)
        };

        // Valor
        if (!decimal.TryParse(cupom.value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
            throw new CupomException(ErrorMessages.CupomInvalid);

        if (v <= 0m) throw new CupomException(ErrorMessages.CupomInvalid);

        // Capar percentual em 100%
        if (type == CupomType.Percentage && v > 100m) v = 100m;

        return (type, v);
    }
   
}
