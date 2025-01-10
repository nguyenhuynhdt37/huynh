using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;
using System.Web;

namespace OnlineCourse.Controllers
{
    public class DocumentController : Controller
    {
        private readonly DataContext _context;
        public DocumentController(DataContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.Documents.OrderBy(d => d.Id).ToListAsync());
        }


    }
}