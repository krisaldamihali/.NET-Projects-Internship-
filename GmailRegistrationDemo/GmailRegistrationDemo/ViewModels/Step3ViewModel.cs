﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace GmailRegistrationDemo.ViewModels
{
    // ViewModel for Step 3: Choose Gmail Address.
    public class Step3ViewModel
    {
        [Required(ErrorMessage = "Gmail address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = null!;

        // Initialize SuggestedEmails to prevent null references
        public List<string> SuggestedEmails { get; set; } = new List<string>();

        // Selected Suggested Email
        public string? SuggestedEmail { get; set; }

        // Custom Email Input 
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Remote(action: "IsEmailAvailable", controller: "RemoteValidation", ErrorMessage = "Email is already in use.")]
        public string? CustomEmail { get; set; }
    }
}