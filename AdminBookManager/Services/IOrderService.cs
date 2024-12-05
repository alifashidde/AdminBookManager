using AdminBookManager.Models;

namespace AdminBookManager.Services
{
    public interface IOrderService
    {
        AdminBookManager.Models.Order CreateOrder(Payment payment, List<Book> purchasedBooks);  // Updated to include books
        AdminBookManager.Models.Order GetOrderById(int orderId);
    }
}
