using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Components
{
    public class BlogCategoriesViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public BlogCategoriesViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var category = await _context.BlogCategories.ToListAsync();
            return await Task.FromResult((IViewComponentResult)View("Default", category));
        }
    }
}