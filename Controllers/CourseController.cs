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
    public class CourseController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPay;
        public CourseController(DataContext context, IConfiguration configuration, IVnPayService vnPay)
        {
            _context = context;
            _configuration = configuration;
            _vnPay = vnPay;
        }

        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Kiểm tra xem người dùng đã đăng ký khóa học này chưa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Chỉ kiểm tra khi id là 2 (theo yêu cầu)
            if (userId != null)
            {
                var isEnrolled = await _context.Enrollments
                    .AnyAsync(e => e.CourseId == id && e.userId == userId);

                if (!isEnrolled)
                {
                    // Người dùng chưa đăng ký khóa học này, chuyển hướng đến trang đăng ký
                    return RedirectToAction("Enroll", new { courseId = id });
                }
            }

            var course = await _context.Courses
                .Include(c => c.Chapters)
                .ThenInclude(c => c.Lessons)
                .Include(c => c.Ratings)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var totalRatings = course.Ratings.Count;

            var starStats = new Dictionary<int, int>();
            for (int star = 5; star >= 1; star--)
            {
                starStats[star] = course.Ratings.Count(r => r.Rate == star);
            }

            var starPercentage = starStats.ToDictionary(
                k => k.Key,
                k => totalRatings > 0 ? (k.Value * 100) / totalRatings : 0
            );

            ViewBag.StarStats = starStats;
            ViewBag.StarPercentage = starPercentage;

            var averageRate = course.Ratings.Count > 0 ? Math.Round(course.Ratings.Average(r => r.Rate), 1) : 0;
            ViewBag.AverageRate = averageRate;

            return View(course);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(int courseId, string comment, int rate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var rating = new Rating
            {
                CourseId = courseId,
                UserId = userId,
                Comment = comment,
                Rate = rate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditRating(int id)
        {
            var review = await _context.Ratings.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != currentUserId)
            {
                return Forbid();
            }

            return View(review);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRating(Rating rating)
        {
            var existingRating = await _context.Ratings.FindAsync(rating.Id);
            if (existingRating == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingRating.UserId != currentUserId)
            {
                return Forbid();
            }

            existingRating.Comment = rating.Comment;
            existingRating.Rate = rating.Rate;
            existingRating.UpdatedAt = DateTime.Now;

            _context.Ratings.Update(existingRating);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Course", new { id = existingRating.CourseId });
        }

        [Authorize]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = await _context.Ratings.FindAsync(id);

            if (review == null || review.UserId != userId)
            {
                return Forbid();
            }

            _context.Ratings.Remove(review);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class SearchViewModel
        {
            public string? keyword { get; set; }
        }
        public async Task<IActionResult> Search(SearchViewModel searchViewModel)
        {
            var courses = await _context.Courses
                .Where(c => c.Name != null && searchViewModel.keyword != null && c.Name.Contains(searchViewModel.keyword))
                .ToListAsync();
            ViewBag.Keyword = searchViewModel.keyword;

            return View(courses);
        }
        [Authorize]
        public async Task<IActionResult> Enroll(int courseId)
        {
            // Tìm khóa học với thông tin chi tiết để hiển thị trước khi đăng ký
            var course = await _context.Courses
                .Include(c => c.Chapters.OrderBy(ch => ch.Order))
                    .ThenInclude(ch => ch.Lessons.OrderBy(l => l.Order))
                .Include(c => c.Lessons.OrderBy(l => l.Order))
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            // Tìm bài học đầu tiên để hiển thị demo
            Lesson? firstLesson = null;

            // Ưu tiên tìm trong các chương
            if (course.Chapters != null && course.Chapters.Count > 0)
            {
                var firstChapter = course.Chapters.OrderBy(c => c.Order).FirstOrDefault();
                if (firstChapter?.Lessons != null && firstChapter.Lessons.Count > 0)
                {
                    firstLesson = firstChapter.Lessons.OrderBy(l => l.Order).FirstOrDefault();
                }
            }

            // Nếu không tìm thấy trong chương, tìm trong danh sách bài học trực tiếp của khóa học
            if (firstLesson == null && course.Lessons != null && course.Lessons.Count > 0)
            {
                firstLesson = course.Lessons.OrderBy(l => l.Order).FirstOrDefault();
            }

            // Tạo ViewModel cho view
            var viewModel = new OnlineCourse.Models.ViewModels.CourseEnrollViewModel
            {
                Course = course,
                FirstLesson = firstLesson,
                CourseId = courseId
            };

            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            // Nếu là khóa học miễn phí (PromotionPrice = 0 hoặc Price = 0 và PromotionPrice = null)
            if ((course.PromotionPrice.HasValue && course.PromotionPrice.Value == 0) ||
                (course.Price.HasValue && course.Price.Value == 0) ||
                (!course.Price.HasValue && !course.PromotionPrice.HasValue))
            {
                // Tạo đăng ký mới cho khóa học miễn phí
                var enrollment = new OnlineCourse.Models.Enrollments
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
                var paymentInformation = new PaymentInformationModel
                {
                    OrderType = "Thanh toán khóa học",
                    Amount = (double)course.PromotionPrice.Value,
                    OrderDescription = $"Thanh toán khóa học {course.Name}",
                    Name = userId
                };
                var paymentUrl = _vnPay.CreatePaymentUrl(paymentInformation, HttpContext);
                var order = new Order
                {
                    UserId = userId,
                    CourseId = courseId,
                    Amount = (decimal)course.PromotionPrice.Value,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    PaymentMethod = "Vnpay",
                    PaymentUrl = paymentUrl
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return Redirect(paymentUrl);
            }
        }
        // 2. Callback khi thanh toán hoàn tất (user OK)
        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var resp = _vnPay.PaymentExecute(Request.Query);
            var order = await _context.Orders.FindAsync(resp.OrderId);
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