using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using System.Diagnostics;

namespace OnlineCourse.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly DataContext _context;

		public HomeController(ILogger<HomeController> logger, DataContext context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			var courses = _context.Courses.Include("Category").Include(c => c.Lessons).ToList();
			return View(courses);
		}
		
		public async Task<IActionResult> AllCourses()
		{
			var allCourses = await _context.Courses.Include(c => c.Lessons).Where(x => x.Status == true).OrderBy(c => c.Id).ToListAsync();
			return View(allCourses);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statuscode)
		{
			if (statuscode == 404)
			{
				return View("404");
			}
			else
			{
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}
		}
	}
}
