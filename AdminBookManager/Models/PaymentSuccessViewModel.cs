namespace AdminBookManager.Models
{
    public class PaymentSuccessViewModel
    {
        public decimal TotalPrice { get; set; }
        public string BookTitle { get; set; }
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
    }

}
