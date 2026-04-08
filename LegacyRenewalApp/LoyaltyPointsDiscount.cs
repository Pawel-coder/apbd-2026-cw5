namespace LegacyRenewalApp;

public class LoyaltyPointsDiscount : IDiscountStrategy
{
    public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount,
        bool useLoyaltyPoints)
    {
        if (!useLoyaltyPoints || customer.LoyaltyPoints <= 0)
            return new DiscountResult(0m, string.Empty); 
        int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
        return new DiscountResult(pointsToUse, $"loyalty points used: {pointsToUse}; ");
    }
}