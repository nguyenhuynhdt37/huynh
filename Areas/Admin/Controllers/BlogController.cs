using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Blog")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly DataContext _context;
        public BlogController(DataContext context)
        {
            _context = context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var categories = _context.BlogCategories.ToList();
            ViewBag.CategoryList = categories;
            
            return View(await _context.Blogs.OrderBy(x => x.BlogId).ToListAsync());
        }

        #region Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.BlogCategoryId = new SelectList(_context.BlogCategories, "BlogCategoryId", "Name");
            return View();
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BlogCategoryId = new SelectList(_context.BlogCategories, "BlogCategoryId", "Name", blog.BlogCategoryId);
            return View(blog);
        }
        #endregion Create

        #region Edit
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewBag.BlogCategoryId = new SelectList(_context.BlogCategories, "BlogCategoryId", "Name", blog.BlogCategoryId);
            return View(blog);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Blog blog)
        {
            if (ModelState.IsValid)
            {
                var existingBlog = await _context.Blogs.FindAsync(blog.BlogId);
                if (existingBlog == null)
                {
                    return NotFound();
                }
                existingBlog.Title = blog.Title;
                existingBlog.Content = blog.Content;
                existingBlog.Image = blog.Image;
                existingBlog.Author = blog.Author;
                existingBlog.Status = blog.Status;
                existingBlog.BlogCategoryId = blog.BlogCategoryId;
                existingBlog.CreatedDate = blog.CreatedDate;
                _context.Blogs.Update(existingBlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            }
            ViewBag.BlogCategoryId = new SelectList(_context.BlogCategories, "BlogCategoryId", "Name", blog.BlogCategoryId);
            return View(blog);
        }
        #endregion Edit

        #region Delete
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Blog blog)
        {
            var existingBlog = await _context.Blogs.FindAsync(blog.BlogId);
            if (existingBlog == null)
            {
                return NotFound();
            }
            _context.Blogs.Remove(existingBlog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion Delete
        
    }
}