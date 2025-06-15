using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Components
{
    [ViewComponent(Name = "Header")]
    public class HeaderViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public HeaderViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfMenu = await _context.CourseCategories.Where(c => c.Status == true).ToListAsync();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfMenu));
        }
    }
}