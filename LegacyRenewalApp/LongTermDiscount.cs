namespace LegacyRenewalApp;

public class LongTermDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (customer.YearsWithCompany >= 5)
            return new DiscountResult(baseAmount * 0.07m, "long-term loyalty discount; ");
        return new DiscountResult(0m,string.Empty);
    }
}