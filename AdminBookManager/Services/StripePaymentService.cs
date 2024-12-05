using Microsoft.Extensions.Configuration;
using Stripe;

namespace AdminBookManager.Services
{
    public class StripePaymentService
    {
        private readonly string _secretKey;

        public StripePaymentService(IConfiguration configuration)
        {
            // Retrieve the secret key from appsettings.json
            _secretKey = configuration.GetValue<string>("Stripe:SecretKey");

            // Set the secret key globally for Stripe
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<Charge> CreateChargeAsync(decimal amount, string source, string currency = "usd")
        {
            var options = new ChargeCreateOptions
            {
                Amount = (long)(amount * 100),  // Stripe expects the amount in cents
                Currency = currency,
                Source = source,  // Token or Card ID
                Description = "Book purchase"
            };

            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options);  // Call Stripe API

            return charge;
        }
    }
}
