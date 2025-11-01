using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Interfaces;
public interface IDiscountCalculator
{
    decimal ComputeDiscount(decimal baseTotal, CupomType type, decimal value);    
}