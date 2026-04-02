namespace LegacyRenewalApp;

public class SilverDiscount : IDiscountStrategy
{
    public decimal CalculateDiscount(decimal baseAmount)
    {
        return baseAmount * 0.05m;
    }
}