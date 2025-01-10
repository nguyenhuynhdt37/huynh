using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.OrderBy(r => r.Id).ToListAsync());
        }

        #region Create Role
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (!_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
                TempData["Success"] = "Role created successfully";
            }
            return RedirectToAction("Index");
        }
        #endregion Create Role

        #region Edit Role
        [HttpGet]
		[Route("Edit")]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}
			var role = await _roleManager.FindByIdAsync(id);
			return View(role);
		}
		[HttpPost]
		[Route("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, IdentityRole model)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				var role = await _roleManager.FindByIdAsync(id);
				if (role == null)
				{ 
					return NotFound();
				}
				role.Name = model.Name;

				try
				{
					await _roleManager.UpdateAsync(role);
					TempData["Success"] = "Role updated successfully";
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}

			}
			return View(model ?? new IdentityRole { Id = id });
			
		}
        #endregion Edit Role

        #region Delete Role
		[HttpGet]
		[Route("Delete")]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				return NotFound();
			}
			return View(role);
		}
        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = await _roleManager.FindByIdAsync(role.Id);
                if (existingRole == null)
                {
                    return NotFound();
                }
                try
                {
                    await _roleManager.DeleteAsync(existingRole);
                    TempData["Success"] = "Role deleted successfully";
                    return RedirectToAction("Index"); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(role);
        }
        #endregion Delete Role


    }
}