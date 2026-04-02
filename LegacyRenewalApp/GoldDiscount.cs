namespace LegacyRenewalApp;

public class GoldDiscount : IDiscountStrategy
{
    public decimal CalculateDiscount(decimal baseAmount)
    {
        return baseAmount * 0.10m;
    }
}