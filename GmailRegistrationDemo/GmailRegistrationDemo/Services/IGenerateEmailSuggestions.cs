namespace GmailRegistrationDemo.Services
{
    public interface IGenerateEmailSuggestions
    {
        Task<List<string>> GenerateUniqueEmailsAsync(string baseEmail, int count = 2);
    }
}