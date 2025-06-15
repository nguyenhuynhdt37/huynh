
using OnlineCourse.Models.ViewModels;

namespace OnlineCourse.Services.Paypal
{
    public interface IPayPalService
    {
        Task<string> CreateOrderAsync(string orderId, decimal amount);
        Task<PaymentResponseModel> CaptureOrderAsync(string orderId);
    }
}