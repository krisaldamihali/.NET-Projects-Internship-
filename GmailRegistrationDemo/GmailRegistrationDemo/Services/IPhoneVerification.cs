using System.Threading.Tasks;

namespace GmailRegistrationDemo.Services
{
    public interface IPhoneVerification
    {
        Task<string> SendVerificationCodeAsync(string phoneNumber);
        bool ValidateCode(string phoneNumber, string code);

        // Optional: for debugging/testing
        void PrintAllCodes();
    }
}
