using System.ComponentModel.DataAnnotations;
namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 6: Enter Verification Code.
    public class Step6ViewModel
    {
        [Required(ErrorMessage = "Verification code is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit code.")]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; } = null!;
    }
}