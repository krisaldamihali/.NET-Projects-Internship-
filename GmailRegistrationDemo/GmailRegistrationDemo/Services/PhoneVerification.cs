using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GmailRegistrationDemo.Services
{
    public class PhoneVerification : IPhoneVerification
    {
        private readonly ILogger<PhoneVerification> _logger;

        public PhoneVerification(ILogger<PhoneVerification> logger)
        {
            _logger = logger;
        }

        private static readonly ConcurrentDictionary<string, VerificationEntry> _verificationCodes = new();

        public async Task<string> SendVerificationCodeAsync(string phoneNumber)
        {
            var code = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            _verificationCodes[phoneNumber] = new VerificationEntry
            {
                Code = code,
                Expiry = expiry,
                Attempts = 0
            };

            await Task.Delay(100); // Simulate sending SMS

            _logger.LogInformation("Verification code for {PhoneNumber}: {Code}", phoneNumber, code);

            return code;
        }

        public bool ValidateCode(string phoneNumber, string code)
        {
            if (_verificationCodes.TryGetValue(phoneNumber, out var entry))
            {
                if (DateTime.UtcNow > entry.Expiry)
                {
                    _verificationCodes.TryRemove(phoneNumber, out _);
                    return false;
                }

                if (entry.Attempts >= 5)
                    return false;

                if (entry.Code == code)
                {
                    _verificationCodes.TryRemove(phoneNumber, out _);
                    return true;
                }

                entry.Attempts++;
                return false;
            }

            return false;
        }

        public void PrintAllCodes()
        {
            foreach (var kvp in _verificationCodes)
            {
                _logger.LogInformation("Phone: {Phone}, Code: {Code}, Expiry: {Expiry}, Attempts: {Attempts}",
                    kvp.Key, kvp.Value.Code, kvp.Value.Expiry, kvp.Value.Attempts);
            }
        }
    }

    public class VerificationEntry
    {
        public string Code { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public int Attempts { get; set; }
    }
}
