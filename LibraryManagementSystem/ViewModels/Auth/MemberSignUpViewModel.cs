using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels.Auth
{
    public class MemberSignUpViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
