using Test.UOL.Web.Interfaces;
using CupomType = Test.UOL.Web.Entities.CupomType;

namespace Test.UOL.Web.Services;

public class DiscountCalculator : IDiscountCalculator
{
    public decimal ComputeDiscount(decimal baseTotal, CupomType type, decimal value)
    {
        if (baseTotal <= 0) return 0m;

        decimal discount = type switch
        {
            CupomType.Percentage => Math.Round(baseTotal * (value / 100m), 2, MidpointRounding.AwayFromZero),
            CupomType.Fixed => value,
            _ => 0m
        };

        if (discount < 0) discount = 0m;
        if (discount > baseTotal) discount = baseTotal;
        return discount;
    }

    
}