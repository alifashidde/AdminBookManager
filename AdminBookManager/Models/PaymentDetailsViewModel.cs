namespace AdminBookManager.Models
{
    [Serializable]
    public class PaymentDetailsViewModel
    {
        public int PaymentId { get; set; }  // Changed to string as per your request
        public int BookId { get; set; }
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }  // Changed to decimal to handle total price properly
        public string PaymentMethod { get; set; }
        public string PaymentToken { get; set; }
        public string PaymentStatus { get; set; } // Added PaymentStatus property
        public DateTime PaymentDate { get; set; } // Added PaymentDate property
        public string StripeChargeId { get; set; }
        public decimal Amount { get; set; }
    }


}
