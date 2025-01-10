using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineCourse.Models;

namespace OnlineCourse.Controllers
{
    public class ProgressController : ControllerBase
    {
             private readonly DataContext _context;
        public ProgressController(DataContext context) {
            _context = context;
        }
//         public async Task<IActionResult> GetProgressAsync()
//         {
//             try {
// var progress = await _context.Progresses.ToListAsync();
//                 return Ok(progress);
//             return Ok();
//             }catch(Exception ex) {
//                 return BadRequest(ex.Message);
//             }
//         }
    }
}