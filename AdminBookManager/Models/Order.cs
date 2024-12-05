namespace AdminBookManager.Models
{
    public class Order
    {
        public int OrderId { get; set; }   // Primary key
        public int? PaymentId { get; set; } // Nullable foreign key to Payment
        public decimal? TotalPrice { get; set; } // Nullable DECIMAL for total price
        public string Status { get; set; }  // NVARCHAR(50) for status

        // Navigation properties
        public Payment Payment { get; set; }
    }

}
