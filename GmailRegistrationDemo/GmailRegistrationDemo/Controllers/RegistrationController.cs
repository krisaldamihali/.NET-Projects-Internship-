using GmailRegistrationDemo.Data;
using GmailRegistrationDemo.Models;
using GmailRegistrationDemo.Services;
using GmailRegistrationDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GmailRegistrationDemo.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly GmailDBContext _context;
        private readonly IGenerateEmailSuggestions _generateSuggestions;
        private readonly IPhoneVerification _phoneVerification;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IRegistrationSessionService _registrationSessionService;
        public RegistrationController(
        GmailDBContext context,
        IGenerateEmailSuggestions generateSuggestions,
        IPhoneVerification phoneVerification,
        ILogger<RegistrationController> logger,
        IRegistrationSessionService registrationSessionService)
        {
            _context = context;
            _generateSuggestions = generateSuggestions;
            _phoneVerification = phoneVerification;
            _logger = logger;
            _registrationSessionService = registrationSessionService;
        }
        [HttpGet]
        public async Task<IActionResult> Step1()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                var model = new Step1ViewModel
                {
                    FirstName = regSession.FirstName ?? string.Empty,
                    LastName = regSession.LastName ?? string.Empty
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step1.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step1(Step1ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regId = Request.Cookies["RegistrationId"];
                    var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                    regSession.FirstName = model.FirstName ?? string.Empty;
                    regSession.LastName = model.LastName ?? string.Empty;
                    regSession.LastUpdated = DateTime.UtcNow;
                    await _registrationSessionService.SaveChangesAsync();
                    return RedirectToAction(nameof(Step2));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step1 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Step2()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                var model = new Step2ViewModel();
                if (regSession.DateOfBirth.HasValue)
                {
                    model.Month = regSession.DateOfBirth.Value.ToString("MMMM");
                    model.Day = regSession.DateOfBirth.Value.Day;
                    model.Year = regSession.DateOfBirth.Value.Year;
                }
                if (regSession.Gender.HasValue)
                {
                    model.Gender = regSession.Gender.Value;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step2.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step2(Step2ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate Month (string)
                    bool validMonth = DateTime.TryParseExact(
                    model.Month,
                    "MMMM",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime monthDate);
                    if (!validMonth)
                    {
                        ModelState.AddModelError(nameof(model.Month), "Please select a valid month.");
                    }
                    else
                    {
                        int monthNumber = monthDate.Month;
                        // Validate Year (non-nullable int)
                        if (model.Year < 1900 || model.Year > DateTime.Now.Year)
                        {
                            ModelState.AddModelError(nameof(model.Year), "Please enter a valid year.");
                        }
                        // Validate Day based on month and year
                        int daysInMonth = DateTime.DaysInMonth(model.Year, monthNumber);
                        if (model.Day < 1 || model.Day > daysInMonth)
                        {
                            ModelState.AddModelError(nameof(model.Day), $"Please enter a valid day (1 to {daysInMonth}).");
                        }
                        // If no validation errors so far
                        if (ModelState.ErrorCount == 0)
                        {
                            DateTime dob = new DateTime(model.Year, monthNumber, model.Day);
                            // Check minimum age 13
                            var today = DateTime.Today;
                            int age = today.Year - dob.Year;
                            if (dob > today.AddYears(-age)) age--;
                            if (age < 13)
                            {
                                ModelState.AddModelError(nameof(model.Year), "You must be at least 13 years old.");
                                return View(model);
                            }
                            var regId = Request.Cookies["RegistrationId"];
                            var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                            regSession.DateOfBirth = dob;
                            regSession.Gender = model.Gender;
                            regSession.LastUpdated = DateTime.UtcNow;
                            await _registrationSessionService.SaveChangesAsync();
                            return RedirectToAction(nameof(Step3));
                        }
                    }
                }
                // Return view with validation messages
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step2 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Step3()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                if (string.IsNullOrWhiteSpace(regSession.FirstName) || string.IsNullOrWhiteSpace(regSession.LastName))
                    return RedirectToAction(nameof(Step1));
                var baseEmail = $"{regSession.FirstName.Trim().ToLowerInvariant()}.{regSession.LastName.Trim().ToLowerInvariant()}@example.com";
                var suggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync(baseEmail, 3);
                var model = new Step3ViewModel
                {
                    SuggestedEmails = suggestedEmails,
                    Email = (regSession.Email ?? baseEmail).Trim()
                };
                if (suggestedEmails.Contains(model.Email))
                    model.SuggestedEmail = model.Email;
                else
                    model.SuggestedEmail = "createOwn";
                if (model.SuggestedEmail == "createOwn")
                    model.CustomEmail = regSession.Email?.Trim();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step3.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step3(Step3ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regId = Request.Cookies["RegistrationId"];
                    var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                    var normalizedEmail = model.Email?.Trim().ToLowerInvariant() ?? string.Empty;
                    var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
                    if (exists)
                    {
                        ModelState.AddModelError("Email", "This email address is already taken. Please choose another.");
                        model.SuggestedEmails = await _generateSuggestions.GenerateUniqueEmailsAsync($"{regSession.FirstName?.Trim().ToLowerInvariant()}.{regSession.LastName?.Trim().ToLowerInvariant()}@example.com", 3);
                        return View(model);
                    }
                    regSession.Email = normalizedEmail;
                    regSession.LastUpdated = DateTime.UtcNow;
                    await _registrationSessionService.SaveChangesAsync();
                    return RedirectToAction(nameof(Step4));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step3 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult Step4()
        {
            try
            {
                return View(new Step4ViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step4.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step4(Step4ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regId = Request.Cookies["RegistrationId"];
                    var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                    regSession.PasswordHash = model.Password ?? string.Empty; // Will hash on final step
                    regSession.LastUpdated = DateTime.UtcNow;
                    await _registrationSessionService.SaveChangesAsync();
                    return RedirectToAction(nameof(Step5));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step4 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Step5()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                var model = new Step5ViewModel
                {
                    CountryCode = regSession.CountryCode ?? string.Empty,
                    PhoneNumber = regSession.PhoneNumber ?? string.Empty,
                    CountryCodes = Step5ViewModel.GetCountryCodes()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step5.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step5(Step5ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regId = Request.Cookies["RegistrationId"];
                    var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                    regSession.CountryCode = model.CountryCode ?? string.Empty;
                    regSession.PhoneNumber = model.PhoneNumber ?? string.Empty;
                    regSession.LastUpdated = DateTime.UtcNow;
                    await _registrationSessionService.SaveChangesAsync();
                    await _phoneVerification.SendVerificationCodeAsync($"{model.CountryCode}{model.PhoneNumber}");
                    return RedirectToAction(nameof(Step6));
                }
                model.CountryCodes = Step5ViewModel.GetCountryCodes();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step5 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult Step6()
        {
            try
            {
                return View(new Step6ViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step6.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step6(Step6ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regId = Request.Cookies["RegistrationId"];
                    var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                    var phone = $"{regSession.CountryCode}{regSession.PhoneNumber}";
                    if (_phoneVerification.ValidateCode(phone, model.VerificationCode ?? string.Empty))
                    {
                        return RedirectToAction(nameof(Step7));
                    }
                    else
                    {
                        ModelState.AddModelError("VerificationCode", "Invalid verification code.");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step6 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Step7()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                var model = new Step7ViewModel
                {
                    RecoveryEmail = regSession.RecoveryEmail ?? string.Empty
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step7.");
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Step7(Step7ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var regId = Request.Cookies["RegistrationId"];
                    var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                    regSession.RecoveryEmail = model.RecoveryEmail ?? string.Empty;
                    regSession.LastUpdated = DateTime.UtcNow;
                    await _registrationSessionService.SaveChangesAsync();
                    return RedirectToAction(nameof(Step8));
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step7 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Step8()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                var model = new Step8ViewModel
                {
                    FullName = $"{regSession.FirstName ?? string.Empty} {regSession.LastName ?? string.Empty}".Trim(),
                    Email = regSession.Email ?? string.Empty,
                    PhoneNumber = $"{regSession.CountryCode ?? string.Empty}{regSession.PhoneNumber ?? string.Empty}",
                    Gender = regSession.Gender?.ToString() ?? string.Empty,
                    RecoveryEmail = regSession.RecoveryEmail ?? string.Empty,
                    DateOfBirth = regSession.DateOfBirth ?? DateTime.MinValue
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step8.");
                return View("Error");
            }
        }
        [HttpPost]
        public IActionResult Step8Confirm()
        {
            try
            {
                return RedirectToAction(nameof(Step9));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step8 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult Step9()
        {
            try
            {
                return View(new Step9ViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Step9.");
                return View("Error");
            }
        }
        [HttpPost]
        public IActionResult Step9(Step9ViewModel model)
        {
            try
            {
                if (ModelState.IsValid && model.Agree)
                {
                    return RedirectToAction(nameof(Step10));
                }
                ModelState.AddModelError("Agree", "You must agree to the terms to proceed.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step9 POST.");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Step10()
        {
            try
            {
                var regId = Request.Cookies["RegistrationId"];
                var regSession = await _registrationSessionService.GetOrCreateSessionAsync(regId);
                if (string.IsNullOrWhiteSpace(regSession.FirstName) ||
                string.IsNullOrWhiteSpace(regSession.LastName) ||
                string.IsNullOrWhiteSpace(regSession.Email) ||
                string.IsNullOrWhiteSpace(regSession.PasswordHash) ||
                !regSession.DateOfBirth.HasValue ||
                !regSession.Gender.HasValue ||
                string.IsNullOrWhiteSpace(regSession.CountryCode) ||
                string.IsNullOrWhiteSpace(regSession.PhoneNumber))
                {
                    return RedirectToAction(nameof(Step1));
                }
                var user = new User
                {
                    FirstName = regSession.FirstName,
                    LastName = regSession.LastName,
                    Email = regSession.Email,
                    DateOfBirth = regSession.DateOfBirth.Value,
                    Gender = regSession.Gender.Value,
                    CountryCode = regSession.CountryCode,
                    PhoneNumber = regSession.PhoneNumber,
                    RecoveryEmail = regSession.RecoveryEmail ?? string.Empty
                };
                // Hash the password using BCrypt
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(regSession.PasswordHash);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                await _registrationSessionService.DeleteSessionAsync(regSession);
                return View("Dashboard", user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Step10.");
                return View("Error");
            }
        }
        public async Task<IActionResult> Dashboard()
        {
            int UserId = 1;
            var user = await _context.Users.FindAsync(UserId);
            return View(user);
        }
    }
}