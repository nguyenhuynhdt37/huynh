using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;
using System.Threading.Tasks;

namespace OnlineCourse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatApiController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public ChatApiController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            var response = await _geminiService.GetChatResponseAsync(request.Message);

            return Ok(new { response });
        }

        [HttpPost("reset")]
        public IActionResult ResetChat()
        {
            _geminiService.ResetChat();
            return Ok(new { success = true });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}