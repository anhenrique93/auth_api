using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TaskCircle.AuthentcationApi.DTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "The curren password is Required")]
        [PasswordPropertyText]
        [MinLength(6, ErrorMessage = "The password must have at least 6 characters.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]+$",
        ErrorMessage = "The password must contain at least one number and one special character.")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "The new password is Required")]
        [PasswordPropertyText]
        [MinLength(6, ErrorMessage = "The password must have at least 6 characters.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]+$",
        ErrorMessage = "The password must contain at least one number and one special character.")]
        public string? NewPassword { get; set; }
    }
}
