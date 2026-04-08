namespace LegacyRenewalApp;

public interface IDiscountStrategy
{
    DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, bool useLoyaltyPoints);
}