using AdminBookManager.Models;

namespace AdminBookManager.Services
{
    public class OrderService : IOrderService
    {
        private readonly AdminBookManagerContext _dbContext;

        public OrderService(AdminBookManagerContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Implement CreateOrder method to accept Payment and List<Book>
        public AdminBookManager.Models.Order CreateOrder(Payment payment, List<Book> purchasedBooks)
        {
            if (payment == null || purchasedBooks == null || !purchasedBooks.Any())
            {
                throw new ArgumentException("Invalid payment or books.");
            }

            decimal totalPrice = 0;
            foreach (var book in purchasedBooks)
            {
                totalPrice += book.Price * book.Quantity;
            }

            var order = new AdminBookManager.Models.Order
            {
                PaymentId = payment.PaymentId,
                TotalPrice = totalPrice,
                Status = "Pending"
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            return order;
        }

        public AdminBookManager.Models.Order GetOrderById(int orderId)
        {
            return _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        }
    }
}
