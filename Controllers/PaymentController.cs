using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using OnlineCourse.Models.ViewModels;
using System.Linq;
using OnlineCourse.Services.Vnpay;
using huynh.Models;

namespace OnlineCourse.Controllers
{
    [Route("Payment")]
    public class PaymentController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPay;
        public PaymentController(DataContext context, IConfiguration configuration, IVnPayService vnPay)
        {
            _context = context;
            _configuration = configuration;
            _vnPay = vnPay;
        }
        [Authorize]
        [HttpPost("ProcessEnroll")]
        public async Task<IActionResult> ProcessEnroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Course", new { id = courseId }) });
            }

            // Kiểm tra xem đã đăng ký chưa
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.userId == userId);

            if (existingEnrollment != null)
            {
                // Đã đăng ký rồi, chuyển hướng đến trang chi tiết khóa học
                return RedirectToAction("Details", new { id = courseId });
            }

            // Kiểm tra giá khóa học
            var course = await _context.Courses.FindAsync(courseId);

            if (course == null)
            {
                return NotFound();
            }
            var transactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            // Nếu là khóa học miễn phí (PromotionPrice = 0 hoặc Price = 0 và PromotionPrice = null)
            if ((course.PromotionPrice.HasValue && course.PromotionPrice.Value == 0) ||
                (course.Price.HasValue && course.Price.Value == 0) ||
                (!course.Price.HasValue && !course.PromotionPrice.HasValue))
            {
                // Tạo đăng ký mới cho khóa học miễn phí
                var enrollment = new Enrollments
                {
                    CourseId = courseId,
                    userId = userId,
                    EnrollmentDate = DateTime.Now
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                // Chuyển hướng đến trang chi tiết khóa học sau khi đăng ký thành công
                return RedirectToAction("Details", new { id = courseId });
            }
            else
            {

                var order = new Order
                {
                    UserId = userId,
                    CourseId = courseId,
                    Amount = (decimal)course.PromotionPrice.Value,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    PaymentMethod = "Vnpay",
                    TransactionId = transactionId
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                var paymentInformation = new PaymentInformationModel
                {
                    PaymentId = transactionId,
                    OrderType = "Thanh toán khóa học",
                    Amount = (double)course.PromotionPrice.Value,
                    OrderDescription = $"Thanh toán khóa học {course.Name}",
                    Name = userId
                };
                var paymentUrl = _vnPay.CreatePaymentUrl(paymentInformation, HttpContext);
                return Redirect(paymentUrl);
            }
        }
        [Authorize]
        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var resp = _vnPay.PaymentExecute(Request.Query);
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.TransactionId == resp.PaymentId);
            if (order == null) return NotFound();

            if (resp.Success && resp.VnPayResponseCode == "00")
            {
                order.Status = "Paid";
                order.TransactionId = resp.TransactionId;
                order.PaymentMethod = "Vnpay";
                order.OrderDate = order.OrderDate;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thanh toán thành công";
                await _context.Enrollments.AddAsync(new Enrollments
                {
                    CourseId = order.CourseId,
                    userId = order.UserId,
                    EnrollmentDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Course", new { id = order.CourseId });
            }
            else
            {
                order.Status = "Failed";
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thanh toán thất bại";
                return RedirectToAction("Details", "Course", new { id = order.CourseId });
            }
        }
        [Authorize]
        [HttpGet("PaymentCancelVnpay")]
        public async Task<IActionResult> PaymentCancelVnpay([FromQuery] long vnp_TxnRef)
        {
            var order = await _context.Orders.FindAsync(vnp_TxnRef);
            if (order != null)
            {
                order.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
            TempData["Message"] = "Đã hủy thanh toán";
            return RedirectToAction("Details", "Course", new { id = order.CourseId });
        }
    }
}