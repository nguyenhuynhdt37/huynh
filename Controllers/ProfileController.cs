using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;

namespace OnlineCourse.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DataContext _context;
        public ProfileController(DataContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
    }
}