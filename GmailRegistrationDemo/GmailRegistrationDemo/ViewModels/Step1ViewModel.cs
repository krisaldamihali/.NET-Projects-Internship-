using System.ComponentModel.DataAnnotations;
namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 1: Enter First and Last Name.
    public class Step1ViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;
    }
}