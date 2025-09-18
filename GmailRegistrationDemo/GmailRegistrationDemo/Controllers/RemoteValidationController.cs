﻿using GmailRegistrationDemo.Data;
using GmailRegistrationDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace GmailRegistrationDemo.Controllers
{
    // Controller responsible for handling remote validation requests.
    public class RemoteValidationController : Controller
    {
        private readonly GmailDBContext _context;
        private readonly IGenerateEmailSuggestions _generateSuggestions;

        public RemoteValidationController(GmailDBContext context, IGenerateEmailSuggestions generateSuggestions)
        {
            _context = context;
            _generateSuggestions = generateSuggestions;
        }

        // Checks if the provided email is available. If not, returns suggestions.
        // Email: The email to validate
        // Returns a JSON result indicating availability or suggestions
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailAvailable(string CustomEmail)
        {
            if (string.IsNullOrWhiteSpace(CustomEmail))
            {
                return Json("Please enter a valid email address.");
            }

            var email = CustomEmail.Trim().ToLowerInvariant();

            // Validate email format
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                return Json("Please enter a valid email address.");
            }

            // Optional: Allowed domains whitelist example
            var allowedDomains = new List<string> { "gmail.com", "yahoo.com", "outlook.com", "example.com", "dotnettutorials.net" }; // Adjust as needed
            var domain = email.Split('@').Last();
            if (!allowedDomains.Contains(domain))
            {
                return Json($"Email domain '{domain}' is not allowed.");
            }

            // Check if email exists (case-insensitive)
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email);

            if (emailExists)
            {
                // Optionally, provide alternative suggestions
                //var suggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync(CustomEmail, 3);
                //var suggestions = string.Join(", ", suggestedEmails);
                //return Json($"This email address is already in use. Try one of these: {suggestions}");
                return Json($"This email address is already in use.");
            }

            // If the email is available
            return Json(true);  // Indicates success to jQuery Unobtrusive Validation
        }
    }
}