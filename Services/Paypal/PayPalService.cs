

using OnlineCourse.Controllers;
using OnlineCourse.Models.ViewModels;
using OnlineCourse.Services.Paypal;
using OnlineCourse.Services.Vnpay;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace ShopMVC.Services.Vnpay
{
    public class PayPalService : IPayPalService
    {
        private readonly PayPalHttpClient _client;
        private readonly IConfiguration _cfg;

        public PayPalService(PayPalHttpClient client, IConfiguration cfg)
        {
            _client = client;
            _cfg = cfg;
        }

        public async Task<string> CreateOrderAsync(string orderId, decimal amount)
        {
            var req = new OrdersCreateRequest();
            req.Prefer("return=representation");
            req.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = _cfg["PayPal:ReturnUrl"],
                    CancelUrl = _cfg["PayPal:CancelUrl"]
                },
                PurchaseUnits = new List<PurchaseUnitRequest> {
                new() {
                    ReferenceId = orderId,
                    AmountWithBreakdown = new AmountWithBreakdown {
                        CurrencyCode = "USD",
                        Value        = amount.ToString("F2")
                    }
                }
            }
            });
            var response = await _client.Execute(req);
            var result = response.Result<Order>();
            return result.Links.First(x => x.Rel == "approve").Href;
        }

        public async Task<PaymentResponseModel> CaptureOrderAsync(string token)
        {
            var req = new OrdersCaptureRequest(token);
            req.RequestBody(new OrderActionRequest());
            var response = await _client.Execute(req);
            var result = response.Result<Order>();

            // Lấy ReferenceId mà bạn đã gán ban đầu
            var referenceId = result.PurchaseUnits[0].ReferenceId;  // chính là order.Id.ToString()

            // Lấy mã giao dịch PayPal
            var capture = result.PurchaseUnits[0].Payments.Captures[0];

            return new PaymentResponseModel
            {
                Success = capture.Status == "COMPLETED",
                TransactionId = capture.Id,
                OrderId = referenceId,    // đây là Order.Id mà bạn muốn
                VnPayResponseCode = capture.Status
            };
        }

    }
}