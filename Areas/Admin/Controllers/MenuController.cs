using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Menu")]
    [Authorize(Roles = "Admin")]
    public class MenuController : Controller
    {
        private readonly DataContext _context;
        public MenuController(DataContext context)
        {
            _context = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminMenus.OrderBy(a => a.AdminMenuId).ToListAsync());
        }

        [Route("Create")]
        public IActionResult Create()
        {
            var menuList = (from m in _context.AdminMenus
                            select new SelectListItem()
                            {
                                Text = (m.ItemLevel == 1) ? m.ItemName : "--" + m.ItemName,
                                Value = m.AdminMenuId.ToString()
                            }).ToList();
            menuList.Insert(0, new SelectListItem());
            ViewBag.menuList = menuList;
            return View();
        }
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(AdminMenu menu)
        {
            if (ModelState.IsValid)
            {
                _context.AdminMenus.Add(menu);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menu);
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var menu = _context.AdminMenus.Find(id);
            if (menu == null)
            {
                return NotFound();
            }
            var menuList = (from m in _context.AdminMenus
                            select new SelectListItem()
                            {
                                Text = (m.ItemLevel == 1) ? m.ItemName : "--" + m.ItemName,
                                Value = m.AdminMenuId.ToString()
                            }).ToList();
            menuList.Insert(0, new SelectListItem());
            ViewBag.menuList = menuList;
            return View(menu);
        }

        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit(AdminMenu menu)
        {
            if (ModelState.IsValid)
            {
                _context.AdminMenus.Update(menu);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menu);
        }

        [HttpGet]
        [Route("Delete/{id:int}")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var del = _context.AdminMenus.Find(id);
            if (del == null)
            {
                return NotFound();
            }
            return View(del);
        }

        [HttpPost]
        [Route("Delete/{id:int}"), ActionName("Delete")]
        public IActionResult Delete(int id, AdminMenu menu)
        {
            System.Console.WriteLine("Day là ID: " + id);
            System.Console.WriteLine("Menu id: " + menu.AdminMenuId);

            if (id != menu.AdminMenuId)
            {
                System.Console.WriteLine($"ID mismatch: {id} != {menu.AdminMenuId}");
                return BadRequest("ID mismatch");
            }
            var del = _context.AdminMenus.FirstOrDefault(a => a.AdminMenuId == id);
            if (del == null)
            {
                System.Console.WriteLine("Khong tim thay!");
                return NotFound("Khong tim thay!");
            }
            _context.AdminMenus.Remove(del);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        
    }
}