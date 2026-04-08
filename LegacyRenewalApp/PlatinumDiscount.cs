namespace LegacyRenewalApp;

public class PlatinumDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, bool useLoyaltyPoints)
    {
        if (customer.Segment != "Platinum")
            return new DiscountResult(0m, string.Empty);
        return new DiscountResult(baseAmount * 0.15m, "platinum discount; ");
    }
}