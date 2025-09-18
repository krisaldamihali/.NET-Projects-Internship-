using Microsoft.AspNetCore.Mvc.Rendering;
using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 5: Enter Phone Number.
    // Implements IValidatableObject for custom phone number validation.
    public class Step5ViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Country code is required.")]
        [Display(Name = "Country")]
        public string CountryCode { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        // List of country codes for the dropdown (populated in controller or view)
        public IEnumerable<SelectListItem>? CountryCodes { get; set; }

        // Custom validation for phone number based on country code using libphonenumber-csharp
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrEmpty(CountryCode))
            {
                results.Add(new ValidationResult("Country code must be selected.", new[] { nameof(CountryCode) }));
                return results;
            }

            if (string.IsNullOrEmpty(PhoneNumber))
            {
                results.Add(new ValidationResult("Phone number is required.", new[] { nameof(PhoneNumber) }));
                return results;
            }

            var phoneUtil = PhoneNumberUtil.GetInstance();

            // Remove whitespace for safety
            var nationalNumber = PhoneNumber.Trim();

            // If user mistakenly adds "+" or "00" prefix, remove it
            if (nationalNumber.StartsWith("+"))
                nationalNumber = nationalNumber.Substring(1);

            if (nationalNumber.StartsWith("00"))
                nationalNumber = nationalNumber.Substring(2);

            // Remove country code prefix if user included it by mistake
            if (nationalNumber.StartsWith(CountryCode.Replace("+", "")))
                nationalNumber = nationalNumber.Substring(CountryCode.Replace("+", "").Length);

            // Map calling code to region
            var callingCodeToRegion = new Dictionary<string, string>
            {
                { "+1", "US" },
                { "+44", "GB" },
                { "+91", "IN" },
                { "+355", "AL" }
                // Add more as needed
            };

            if (!callingCodeToRegion.TryGetValue(CountryCode, out var regionCode))
            {
                results.Add(new ValidationResult("Unsupported country code.", new[] { nameof(CountryCode) }));
                return results;
            }

            try
            {
                // Parse expects only the national number and the region code
                var parsedNumber = phoneUtil.Parse(nationalNumber, regionCode);

                if (!phoneUtil.IsValidNumber(parsedNumber))
                {
                    results.Add(new ValidationResult("Please enter a valid phone number.", new[] { nameof(PhoneNumber) }));
                }
            }
            catch (NumberParseException)
            {
                results.Add(new ValidationResult("Invalid phone number format.", new[] { nameof(PhoneNumber) }));
            }

            return results;
        }

        // Static method to provide country codes list for dropdowns
        public static List<SelectListItem> GetCountryCodes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "+1", Text = "United States (+1)" },
                new SelectListItem { Value = "+44", Text = "United Kingdom (+44)" },
                new SelectListItem { Value = "+91", Text = "India (+91)" },
                new SelectListItem { Value = "+355", Text = "Albania (+355)" },
                // Add more countries as needed
            };
        }
    }
}