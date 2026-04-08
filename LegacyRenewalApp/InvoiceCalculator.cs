using System;
using System.Collections.Generic;

namespace LegacyRenewalApp;

public class InvoiceCalculator
{
    private readonly IEnumerable<IDiscountStrategy> _discountStrategy;

    public InvoiceCalculator(IEnumerable<IDiscountStrategy> discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public InvoiceCalculatorResult Calculate(
        Customer customer,
        SubscriptionPlan subscriptionPlan,
        string normalizedPlanCode,
        string normalizedPaymentMethod,
        int seatCount,
        bool inculdePremiumSupport,
        bool useLoyalityPoints)
    {
        var result = new InvoiceCalculatorResult();
        var notes = string.Empty;
        result.BaseAmount = (subscriptionPlan.MonthlyPricePerSeat * seatCount * 12m) + subscriptionPlan.SetupFee;

        foreach (var strategy in _discountStrategy)
        {
            var discount = strategy.CalculateDiscount(customer, subscriptionPlan, seatCount, result.BaseAmount,
                useLoyalityPoints);
            if (discount.Amount > 0)
            {
                result.DiscountAmount += discount.Amount;
                notes += discount.Note;
            }
        }
        
        decimal subtotalAfterDiscount = result.BaseAmount - result.DiscountAmount;
        if (subtotalAfterDiscount < 300m)
        {
            subtotalAfterDiscount = 300m;
            notes += "minimum discounted subtotal applied; ";
        }

        if (inculdePremiumSupport)
        {
            result.SupportFee = GetSupportFee(normalizedPlanCode);
            if (result.SupportFee > 0) notes+="premium support included; ";
        }
        result.PaymentFee = GetPaymentFee(normalizedPaymentMethod, subtotalAfterDiscount + result.SupportFee);
        notes+=GetPaymentFeeNote(normalizedPaymentMethod);
        
        decimal taxRate = GetTaxRate(customer.Country);
        decimal taxBase = subtotalAfterDiscount + result.SupportFee + result.PaymentFee;
        result.TaxAmount = taxBase * taxRate;
        
        result.FinalAmount = taxBase + result.TaxAmount;
        if (result.FinalAmount < 500m)
        {
            result.FinalAmount = 500m;
            notes += "minimum invoice amount applied; ";
        }
        
        result.Notes = notes;
        return result;
    }
    private decimal GetSupportFee(string planCode) => planCode switch
    {
        "START" => 250m,
        "PRO" => 400m,
        "ENTERPRISE" => 700m,
        _ => 0m
    };
    private decimal GetPaymentFee(string method, decimal baseForFee) => method switch
    {
        "CARD" => baseForFee * 0.02m,
        "BANK_TRANSFER" => baseForFee * 0.01m,
        "PAYPAL" => baseForFee * 0.035m,
        "INVOICE" => 0m,
        _ => throw new ArgumentException("Unsupported payment method")
    };
    private string GetPaymentFeeNote(string method) => method switch
    {
        "CARD" => "card payment fee; ",
        "BANK_TRANSFER" => "bank transfer fee; ",
        "PAYPAL" => "paypal fee; ",
        "INVOICE" => "invoice payment; ",
        _ => string.Empty
    };
    private decimal GetTaxRate(string country) => country switch
    {
        "Poland" => 0.23m,
        "Germany" => 0.19m,
        "Czech Republic" => 0.21m,
        "Norway" => 0.25m,
        _ => 0.20m
    };
}