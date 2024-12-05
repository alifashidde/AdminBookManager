using AdminBookManager.Models;

namespace AdminBookManager.Services
{
    public interface IPaymentService
    {
        // Define the method signature
        Payment GetPaymentById(int paymentId);

    }

}
