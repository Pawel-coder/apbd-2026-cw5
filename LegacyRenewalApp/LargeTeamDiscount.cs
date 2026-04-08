namespace LegacyRenewalApp;

public class LargeTeamDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (seatCount >= 50)
        {
            return new DiscountResult(baseAmount * 0.12m, "large team discount; ");
        }
        return new DiscountResult(0m, string.Empty);
    }
}