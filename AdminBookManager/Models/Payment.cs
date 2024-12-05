namespace AdminBookManager.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }  // Primary key
        public string BookTitle { get; set; } // Book title associated with the payment
        public decimal Price { get; set; }    // Price per book
        public int Quantity { get; set; }     // Quantity of books purchased
        public decimal TotalPrice { get; set; } // Total price of the payment
        public string PaymentMethod { get; set; }  // Payment method
        public string PaymentToken { get; set; }   // Payment token (optional)
        public string PaymentStatus { get; set; }  // Payment status (default: 'Pending')
        public DateTime PaymentDate { get; set; }  // Payment date (default: current date)

        // Navigation property for Orders (assuming one payment can be related to many orders)
        public ICollection<Order> Orders { get; set; }
    }

}
