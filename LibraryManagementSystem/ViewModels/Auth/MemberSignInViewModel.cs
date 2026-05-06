using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels.Auth
{
    public class MemberSignInViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
