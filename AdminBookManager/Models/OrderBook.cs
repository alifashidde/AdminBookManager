namespace AdminBookManager.Models
{
    public class OrderBook
    {
        public int OrderId { get; set; }  // Foreign key to Order
        public int BookId { get; set; }   // Foreign key to Book

        // Navigation properties
        public Order Order { get; set; }
        public Book Book { get; set; }
    }

}
