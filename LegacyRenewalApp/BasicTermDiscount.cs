namespace LegacyRenewalApp;

public class BasicTermDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (customer.YearsWithCompany >= 2 && customer.YearsWithCompany < 5)
            return new DiscountResult(baseAmount * 0.03m, "basic loyalty discount; ");
        return new DiscountResult(0m,string.Empty);
    }
}