using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels.Auth
{
    public class AdminSignInViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
