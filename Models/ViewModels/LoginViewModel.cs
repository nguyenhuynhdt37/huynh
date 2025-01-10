using System.ComponentModel.DataAnnotations;

namespace OnlineCourse.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username or Email is required")]
		public string UsernameOrEmail { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }

}