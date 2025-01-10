using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.Models;

namespace OnlineCourse.Controllers
{
    public class ProgressController : ControllerBase
    {
        private readonly DataContext _context;
        public ProgressController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("api/progress/{courseId}")]

        public async Task<IActionResult> GetProgressAsync([FromRoute] int courseId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var processCount = 0;
                var totalLessons = 0;
                var lessons = await _context.Lessons.Where(p => p.CourseId == courseId).ToListAsync();
                if (!lessons.Any()) totalLessons = 0;
                var processes = await _context.Progresses.Where(p => p.CourseId == courseId && p.UserId == userId).ToListAsync();
                if (!processes.Any()) processCount = 0;
                else
                {
                    processCount = processes.Count;
                }
                return Ok(
                    new
                    {
                        totalLessons = lessons.Count,
                        completedLessons = processCount
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}