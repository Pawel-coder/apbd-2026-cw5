using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly IBillingGateway _billingGateway;
        private readonly DiscountCalculator _discountCalculator;

        public SubscriptionRenewalService() : this(
            new NewBillingGateway(),
            new DiscountCalculator(new List<IDiscountStrategy> 
            {
                new SilverDiscount(),
                new GoldDiscount(),
                new PlatinumDiscount(),
                new EducationDiscount(),
                new LongTermDiscount(),
                new BasicTermDiscount(),
                new LargeTeamDiscount(),
                new MediumTeamDiscount(),
                new SmallTeamDiscount()
            }))
        {
        }
        public SubscriptionRenewalService(
            IBillingGateway billingGateway,
            DiscountCalculator discountCalculator)
        {
            _billingGateway = billingGateway;
            _discountCalculator = discountCalculator;
        }
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            ValidateInputs(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customerRepository = new CustomerRepository();
            var planRepository = new SubscriptionPlanRepository();

            var customer = customerRepository.GetById(customerId);
            var plan = planRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            var discount = _discountCalculator.Calculate(customer, plan, normalizedPlanCode, normalizedPaymentMethod,
                seatCount, includePremiumSupport, useLoyaltyPoints);

            var invoice = CreateInvoice(customer, discount, normalizedPlanCode, normalizedPaymentMethod, seatCount);

            _billingGateway.SaveInvoice(invoice);
            SendMessage(customer, invoice,normalizedPlanCode);
            return invoice;
        }
        private void ValidateInputs(int customerId, string planCode, int seatCount, string paymentMethod)
        {
            if (customerId <= 0) throw new ArgumentException("Customer id must be positive");
            if (string.IsNullOrWhiteSpace(planCode)) throw new ArgumentException("Plan code is required");
            if (seatCount <= 0) throw new ArgumentException("Seat count must be positive");
            if (string.IsNullOrWhiteSpace(paymentMethod)) throw new ArgumentException("Payment method is required");
        }

        private void SendMessage(Customer customer, RenewalInvoice invoice, string normalizedPlanCode)
        {
            if (string.IsNullOrWhiteSpace(customer.Email)) return;

            string subject = "Subscription renewal invoice";
            string body = $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                          $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            _billingGateway.SendEmail(customer.Email, subject, body);
        }
        private RenewalInvoice CreateInvoice(
            Customer customer, 
            DiscountCalculatorResult result, 
            string normalizedPlanCode, 
            string normalizedPaymentMethod, 
            int seatCount)
        {
            return new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customer.Id}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(result.BaseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(result.DiscountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(result.SupportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(result.PaymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(result.TaxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(result.FinalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = result.Notes,
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}
