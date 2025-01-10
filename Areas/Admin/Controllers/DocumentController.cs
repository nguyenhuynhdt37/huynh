using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Document")]
    [Authorize(Roles = "Admin")]
    public class DocumentController : Controller
    {
        private readonly DataContext _context;
        public DocumentController(DataContext context)
        {
            _context = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Documents.OrderBy(d => d.Id).ToListAsync());
        }

        #region Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name");
            ViewBag.CategoryId = new SelectList(_context.CourseCategories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document)
        {
            if (ModelState.IsValid)
            {
                _context.Add(document);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The document has been added successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "The document could not be added";
                List<string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            // return View(document);
        }
        #endregion Create

        #region Edit
        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                TempData["Error"] = "The document does not exist";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", document.CourseId);
            ViewBag.CategoryId = new SelectList(_context.CourseCategories, "Id", "Name", document.CategoryId);
            return View(document);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                var existingDocument = await _context.Documents.FindAsync(document.Id);
                if (existingDocument == null)
                {
                    TempData["Error"] = "The document does not exist";
                    return RedirectToAction(nameof(Index));
                }
                existingDocument.Name = document.Name;
                existingDocument.Description = document.Description;
                existingDocument.Type = document.Type;
                existingDocument.FilePath = document.FilePath;
                existingDocument.Link = document.Link;
                existingDocument.Status = document.Status;
                existingDocument.CourseId = document.CourseId;
                existingDocument.CategoryId = document.CategoryId;

                await _context.SaveChangesAsync();
                TempData["Success"] = "The document has been updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "The document could not be updated";
                List<string> errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            // return View(document);
        }
        #endregion Edit

        #region Delete
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                TempData["Error"] = "The document does not exist";
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }
        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Document document)
        {
            var existingDocument = await _context.Documents.FindAsync(document.Id);
            if (existingDocument == null)
            {
                TempData["Error"] = "The document does not exist";
                return RedirectToAction(nameof(Index));
            }
            _context.Documents.Remove(existingDocument);
            await _context.SaveChangesAsync();
            TempData["Success"] = "The document has been deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion Delete
    }
}