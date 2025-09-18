namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 8: Review Account Information.
    public class Step8ViewModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? RecoveryEmail { get; set; }
    }
}