using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;

namespace OnlineCourse.Areas.Admin.Components
{
    [ViewComponent(Name = "AdminMenu")]
    public class AdminMenuComponent : ViewComponent
    {
        private readonly DataContext _context;
        public AdminMenuComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = _context.AdminMenus.Where(m => m.IsActive == true).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", menus));
        }
    }
}