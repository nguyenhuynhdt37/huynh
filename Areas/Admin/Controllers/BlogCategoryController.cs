using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/BlogCategory")]
    [Authorize(Roles = "Admin")]
    public class BlogCategoryController : Controller
    {
        private readonly DataContext _context;
        public BlogCategoryController(DataContext context)
        {
            _context = context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogCategories.OrderBy(c => c.BlogCategoryId).ToListAsync());
        }

        #region create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCategory model)
        {
            if (ModelState.IsValid)
            {
                _context.BlogCategories.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion create

        #region edit
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.BlogCategories.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogCategory model)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.BlogCategories.FindAsync(model.BlogCategoryId);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                existingCategory.Name = model.Name;
				existingCategory.Slug = model.Slug;
				existingCategory.Status = model.Status;

				_context.BlogCategories.Update(existingCategory);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion edit

        #region delete
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.BlogCategories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			_context.BlogCategories.Remove(category);
			await _context.SaveChangesAsync();
			TempData["Success"] = "The category has been deleted successfully";
			return RedirectToAction(nameof(Index));
        }
        #endregion delete
    }
}