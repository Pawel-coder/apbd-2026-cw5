using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly IBillingGateway _billingGateway;
        private readonly InvoiceCalculator _invoiceCalculator;
        private readonly CustomerRepository _customerRepository;
        private readonly SubscriptionPlanRepository _subscriptionPlanRepository;
        public SubscriptionRenewalService() : this(
            new NewBillingGateway(),
            new InvoiceCalculator(new List<IDiscountStrategy> 
            {
                new SilverDiscount(),
                new GoldDiscount(),
                new PlatinumDiscount(),
                new EducationDiscount(),
                new LongTermDiscount(),
                new BasicTermDiscount(),
                new LargeTeamDiscount(),
                new MediumTeamDiscount(),
                new SmallTeamDiscount(),
                new LoyaltyPointsDiscount()
            }),
            new CustomerRepository(),
            new SubscriptionPlanRepository()) {}
        public SubscriptionRenewalService(
            IBillingGateway billingGateway,
            InvoiceCalculator invoiceCalculator,
            CustomerRepository customerRepository,
            SubscriptionPlanRepository subscriptionPlanRepository)
        {
            _billingGateway = billingGateway;
            _invoiceCalculator = invoiceCalculator;
            _customerRepository = customerRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
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

            var customer = _customerRepository.GetById(customerId);
            var plan = _subscriptionPlanRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            var result = _invoiceCalculator.Calculate(customer, plan, normalizedPlanCode, normalizedPaymentMethod,
                seatCount, includePremiumSupport, useLoyaltyPoints);

            var invoice = CreateInvoice(customer, result, normalizedPlanCode, normalizedPaymentMethod, seatCount);

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
            InvoiceCalculatorResult result, 
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
                Notes = result.Notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}
