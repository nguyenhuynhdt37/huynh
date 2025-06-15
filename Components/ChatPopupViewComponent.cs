using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;

namespace OnlineCourse.Components
{
    public class ChatPopupViewComponent : ViewComponent
    {
        private readonly GeminiService _geminiService;

        public ChatPopupViewComponent(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}