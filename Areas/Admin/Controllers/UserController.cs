using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Route("Admin/User")]
	[Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private void AddIdentityErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

        [HttpGet]
		[Route("Index")]
        public async Task<IActionResult> Index()
        {
            var userWithRoles = await (from u in _context.Users
									   join ur in _context.UserRoles on u.Id equals ur.UserId
									   join r in _context.Roles on ur.RoleId equals r.Id
									   select new {User = u, RoleName = r.Name})
                                       .ToListAsync();

			return View(userWithRoles);

        }

		[HttpGet]
		[Route("AllUsers")]
		public async Task<IActionResult> AllUsers()
		{
			return View(await _context.Users.ToListAsync());
		}

        #region Create User
        [HttpGet]
		[Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
				if (createUserResult.Succeeded)
				{
					var createdUser = await _userManager.FindByEmailAsync(user.Email);
					var userId = createdUser.Id;
					var role = await _roleManager.FindByIdAsync(user.RoleId);
					// Add user to role
                    var addRoleResult = await _userManager.AddToRoleAsync(createdUser, role.Name);
					if (addRoleResult.Succeeded)
					{
						return RedirectToAction("Index", "User");
					}
					else
					{
						AddIdentityErrors(addRoleResult);
					}
				}
				else
				{
					AddIdentityErrors(createUserResult);
					return View(user);
				}
            }
            else
            {
                TempData["Error"] = "Có 1 vài lỗi xảy ra";
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
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        #endregion Create User
 
        #region Edit User
        [HttpGet]
		[Route("Edit")]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");
			return View(user);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Edit")]
		public async Task<IActionResult> Edit(AppUserModel model)
		{
			var existingUser = await _userManager.FindByIdAsync(model.Id);// Get user by id
			if (existingUser == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				existingUser.Avatar = model.Avatar;
				existingUser.UserName = model.UserName;
				existingUser.Email = model.Email;
				existingUser.PhoneNumber = model.PhoneNumber;
				existingUser.RoleId = model.RoleId;

				var updateResult = await _userManager.UpdateAsync(existingUser);
				if (updateResult.Succeeded)
				{
					return RedirectToAction("Index", "User");
				}
				else
				{
					AddIdentityErrors(updateResult);
					return View(existingUser);
				}
			}
			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");

			// Model validation failed
			TempData["Error"] = "Model validation failed";
			var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
			string errorMessage = string.Join("\n", errors);

			return View(existingUser);
		}
		
        #endregion Edit User

        #region Delete User
        [HttpGet]
		[Route("Delete")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
		[Route("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var del = await _context.Users.FindAsync(id);
            if (del == null)
            {
                return NotFound();
            }
            _context.Users.Remove(del);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion Delete User

        
    }

}