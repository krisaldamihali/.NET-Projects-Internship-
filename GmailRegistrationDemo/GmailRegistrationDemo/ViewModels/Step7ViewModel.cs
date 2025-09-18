using System.ComponentModel.DataAnnotations;
namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 7: Add Recovery Email.
    public class Step7ViewModel
    {
        [EmailAddress(ErrorMessage = "Please enter a valid recovery email address.")]
        [Display(Name = "Recovery Email (Optional)")]
        public string? RecoveryEmail { get; set; }
    }
}