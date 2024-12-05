namespace AdminBookManager.Models
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }   // Indicates whether the payment was successful
        public string Message { get; set; }    // Message related to the payment result (e.g., success or error message)
    }

}
