using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;

namespace OnlineCourse.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public FooterViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}