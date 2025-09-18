using System.ComponentModel.DataAnnotations;
using GmailRegistrationDemo.Models;
namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 2: Enter Date of Birth and Gender.
    public class Step2ViewModel
    {
        [Required(ErrorMessage = "Month is required.")]
        public string Month { get; set; } = null!;

        [Required(ErrorMessage = "Day is required.")]
        [Range(1, 31, ErrorMessage = "Please enter a valid day.")]
        public int Day { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, 2100, ErrorMessage = "Please enter a valid year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }
    }
}