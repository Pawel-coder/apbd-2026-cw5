namespace LegacyRenewalApp;

public interface IDiscountStrategy
{
    decimal CalculateDiscount(decimal orderAmount);
}