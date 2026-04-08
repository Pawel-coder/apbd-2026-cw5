namespace LegacyRenewalApp;

public class EducationDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount, bool useLoyaltyPoints)
    {
        return (customer.Segment!="Education" || !plan.IsEducationEligible) ?  new DiscountResult(0m, string.Empty):new DiscountResult(baseAmount * 0.20m, "education discount; ");
    }
}