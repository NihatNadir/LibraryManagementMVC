using System.ComponentModel.DataAnnotations;
using LibraryManagementMVC.Data;

namespace LibraryManagementMVC.Models
{
    public class SignInViewModel()
    {
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }
    }
}
