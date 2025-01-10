using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;
using OnlineCourse.Models.ViewModels;

public class AccountController : Controller
{
    private readonly UserManager<AppUserModel> _userManager;
    private readonly SignInManager<AppUserModel> _signInManager;

    public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return Redirect(returnUrl ?? "/");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Quay lại form đăng nhập nếu dữ liệu không hợp lệ
        }

        var user = await _userManager.FindByNameAsync(model.UsernameOrEmail)
                    ?? await _userManager.FindByEmailAsync(model.UsernameOrEmail);

        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                // Nếu ReturnUrl trống hoặc không hợp lệ, chuyển hướng về trang chủ
                if (string.IsNullOrEmpty(model.ReturnUrl) || !Url.IsLocalUrl(model.ReturnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }

                return Redirect(model.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Sai mật khẩu, vui lòng thử lại.");
            }
        }
        else
        {
            ModelState.AddModelError("", "Tên người dùng hoặc email không tồn tại.");
        }

        return View(model); // Quay lại trang đăng nhập nếu có lỗi
    }

    // Register GET
    public IActionResult Register()
    {
        return View();
    }

    // Register POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid) 
        {
            AppUserModel newUser = new AppUserModel 
            { 
                UserName = model.Username, 
                Email = model.Email 
            };

			IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
            
            if (result.Succeeded)
            {
                // await _signInManager.SignInAsync(model, isPersistent: false);
                return RedirectToAction("Login");
                // return Redirect("/Account/Login");
            }
            
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    // Logout
    public async Task<IActionResult> Logout(string returnUrl = "/")
    {
        await _signInManager.SignOutAsync();
        return Redirect(returnUrl);
    }
}
