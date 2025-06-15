using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using OnlineCourse.Models.ViewModels;
using System.Linq;
using OnlineCourse.Services.Vnpay;
using huynh.Models;
using OnlineCourse.Services.Paypal;

namespace OnlineCourse.Controllers
{
    [Route("PaymentPayPal")]
    public class PaymentPayPalController : Controller
    {
        private readonly DataContext _db;
        private readonly IPayPalService _pp;
        public PaymentPayPalController(DataContext db, IPayPalService pp)
        {
            _db = db; _pp = pp;
        }

        [HttpPost("ProcessEnroll")]
        public async Task<IActionResult> ProcessEnroll(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Account",
                   new { returnUrl = Url.Action("Details", "Course", new { id = courseId }) });

            var enrollment = await _db.Enrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.userId == userId);
            if (enrollment != null)
                return RedirectToAction("Details", "Course", new { id = courseId });

            var course = await _db.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound();

            if (course.PromotionPrice.HasValue && course.PromotionPrice.Value == 0)
                return RedirectToAction("Details", "Course", new { id = courseId });
            var transactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.CourseId == courseId && o.UserId == userId);
            if (order != null)
            {
                if (order.Status == "Paid")
                {

                    return RedirectToAction("Details", "Course", new { id = courseId });
                }
                else
                {
                    order.TransactionId = transactionId;
                    order.Amount = (decimal)(await _db.Courses.FindAsync(courseId)).PromotionPrice;
                    order.Status = "Pending";
                    order.PaymentMethod = "PayPal";
                    order.OrderDate = DateTime.UtcNow;
                    _db.Orders.Update(order);
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                order = new Order
                {
                    UserId = userId,
                    CourseId = courseId,
                    Amount = (decimal)(await _db.Courses.FindAsync(courseId)).PromotionPrice,
                    OrderDate = DateTime.UtcNow,
                    TransactionId = transactionId,
                    Status = "Pending",
                    PaymentMethod = "PayPal"
                };
                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();
            }





            // Tạo order Pending

            // Tạo PayPal order và redirect
            var url = await _pp.CreateOrderAsync(transactionId, order.Amount);
            return Redirect(url);
        }

        [HttpGet("PayPalCallback")]
        public async Task<IActionResult> PayPalCallback(string token)
        {
            // token chính là orderId (ReferenceId)
            var resp = await _pp.CaptureOrderAsync(token);
            if (resp.OrderId != null && resp.Success == true)
            {
                var order = await _db.Orders.FirstOrDefaultAsync(o => o.TransactionId == resp.OrderId);
                if (order != null)
                {
                    order.TransactionId = resp.TransactionId;
                    order.Status = resp.Success ? "Paid" : "Failed";
                    order.PaymentUrl = token;
                    await _db.SaveChangesAsync();
                    _db.Enrollments.Add(new Enrollments
                    {
                        CourseId = order.CourseId,
                        userId = order.UserId,
                        EnrollmentDate = DateTime.UtcNow
                    });
                    await _db.SaveChangesAsync();
                    TempData["Message"] = "Thanh toán PayPal thành công";
                    return RedirectToAction("Details", "Course", new { id = order.CourseId });
                }
                else
                {
                    return BadRequest("Order không tồn tại" + resp.OrderId + " " + resp.TransactionId);
                }
            }
            else
            {
                TempData["Message"] = "Thanh toán PayPal thất bại";
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("PayPalCancel")]
        public IActionResult PayPalCancel()
        {
            TempData["Message"] = "Bạn đã huỷ thanh toán PayPal";
            return RedirectToAction("Details", "Course");
        }
    }
}