namespace LegacyRenewalApp;

public class Platinum : IDiscountStrategy
{
    public decimal CalculateDiscount(decimal baseAmount)
    {
        return baseAmount * 0.15m;
    }
}