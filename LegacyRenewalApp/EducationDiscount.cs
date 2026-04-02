namespace LegacyRenewalApp;

public class EducationDiscount : IDiscountStrategy
{
    public decimal CalculateDiscount(decimal baseAmount)
    {
        return baseAmount * 0.20m;
    }
}