namespace LegacyRenewalApp;

public class MediumTeamDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (seatCount >= 20 && seatCount < 50)
        {
            return new DiscountResult(baseAmount * 0.08m, "medium team discount; ");
        }
        return new DiscountResult(0m, string.Empty);
    }
}