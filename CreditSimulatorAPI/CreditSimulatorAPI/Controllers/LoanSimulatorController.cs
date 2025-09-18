using CreditSimulatorAPI.Data;
using CreditSimulatorAPI.Models;
using CreditSimulatorAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreditSimulatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanSimulatorController : ControllerBase
    {
        private readonly LoanCalculatorService _calculator;
        private readonly CreditDbContext _context;

        public LoanSimulatorController(CreditDbContext context)
        {
            _calculator = new LoanCalculatorService();
            _context = context;
        }

        [HttpPost("simulate")]
        public IActionResult SimulateLoan([FromBody] LoanRequest loanRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Llogaritjet përmes shërbimit ekzistues
            decimal monthlyPayment = _calculator.CalculateMonthlyPayment(loanRequest);
            List<PaymentScheduleItem> schedule = _calculator.GeneratePaymentSchedule(loanRequest);

            // 2. Llogarit kohëzgjatjen (në muaj) nga MaturityDate
            int durationMonths = ((loanRequest.MaturityDate.Year - DateTime.Now.Year) * 12) +
                                 (loanRequest.MaturityDate.Month - DateTime.Now.Month);

            if (durationMonths <= 0)
                return BadRequest("Maturity date must be in the future.");

            // 3. Ruaj Loan në DB
            var loan = new Loan
            {
                UserId = 1, // për momentin hardcoded, mund të vijë nga LoanRequest më vonë
                Amount = loanRequest.Principal,
                InterestRate = (decimal)loanRequest.AnnualInterestRate,
                DurationMonths = durationMonths,
                RemainingBalance = loanRequest.Principal
            };

            _context.Loans.Add(loan);
            _context.SaveChanges();

            // 4. Kthe rezultatet
            return Ok(new
            {
                LoanId = loan.Id,
                MonthlyPayment = monthlyPayment,
                PaymentSchedule = schedule,
                RemainingBalance = loan.RemainingBalance
            });
        }
    }
}
