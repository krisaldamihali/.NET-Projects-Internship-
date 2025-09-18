using GmailRegistrationDemo.Models;
namespace GmailRegistrationDemo.Services
{
    public interface IRegistrationSessionService
    {
        Task<RegistrationSession> GetOrCreateSessionAsync(string? registrationId);
        Task<RegistrationSession> CreateNewSessionAsync();
        Task SaveChangesAsync();
        Task DeleteSessionAsync(RegistrationSession session);
    }
}