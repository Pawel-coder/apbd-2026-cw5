using System.Reflection.Metadata.Ecma335;

namespace LegacyRenewalApp;

public class SilverDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, bool useLoyaltyPoints)
    {
        if (customer.Segment != "Silver")
            return new DiscountResult(0m, string.Empty);
        return new DiscountResult(baseAmount * 0.05m,"silver discount; ");
    }
}