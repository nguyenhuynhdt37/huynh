using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(string Slug = "")
        {
            var category = _context.CourseCategories.FirstOrDefault(c => c.Slug == Slug);
			if (category == null)
			{
				return RedirectToAction("Index");
			}
			var products = _context.Courses.Include(c => c.Lessons).Where(p => p.CategoryId == category.Id);

            ViewBag.BlogCategoryList = category;
            ViewBag.CategoryName = category.Name;

			return View(await products.OrderBy(c => c.Id).ToListAsync());
        }

    }
}