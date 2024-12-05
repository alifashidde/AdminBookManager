using Microsoft.EntityFrameworkCore;
using AdminBookManager.Models;
using System.Linq;

namespace AdminBookManager.Services
{
    public class PaymentService
    {
        private readonly AdminBookManagerContext _dbContext;  // Consistent use of _dbContext

        // Constructor accepts the injected DbContext instance
        public PaymentService(AdminBookManagerContext dbContext)
        {
            _dbContext = dbContext; // Initializes the _dbContext field
        }

        // Method to get payment details
        public PaymentDetailsViewModel GetPaymentDetails(int paymentId)
        {
            var payment = _dbContext.Payments
                .FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment == null)
            {
                return null;
            }

            return new PaymentDetailsViewModel
            {
                PaymentId = payment.PaymentId,
                Amount = payment.TotalPrice,
                PaymentMethod = payment.PaymentMethod,
             
            };
        }

        public Payment GetPaymentById(int paymentId)
        {
            return _dbContext.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
        }

        public async Task<PaymentResult> ProcessPayment(Payment payment, string paymentToken, string paymentMethod)
        {
            // Payment processing logic here 
            var result = new PaymentResult();

            // Example payment processing logic
            try
            {

                result.IsSuccess = true;  // set based on whether payment is successful
                result.Message = "Payment Successful";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = $"Payment failed: {ex.Message}";
            }

            return result;
        }
        // Corrected method to save payment details to the database
        public void SavePayment(Payment payment)
        {
            // Save payment details to the database using _dbContext (not _context)
            _dbContext.Payments.Add(payment); // Assuming the table is called 'Payments'
            _dbContext.SaveChanges();
        }
    }
}
