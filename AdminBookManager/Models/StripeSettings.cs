namespace AdminBookManager.Models
{
    // Create a StripeSettings class to bind to appsettings.json
    public class StripeSettings
    {
        public string PublishableKey { get; set; }
        public string SecretKey { get; set; }
    }

}
