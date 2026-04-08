namespace LegacyRenewalApp;

public class SmallTeamDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (seatCount >= 10 && seatCount < 20)
        {
            return new DiscountResult(baseAmount * 0.04m, "small team discount; ");
        }
        return new DiscountResult(0m, string.Empty);
    }
}