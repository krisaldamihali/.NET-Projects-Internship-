using CreditSimulatorAPI.Data;
using CreditSimulatorAPI.Models;
using CreditSimulatorAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditSimulatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanSimulatorController : ControllerBase
    {
        private readonly LoanCalculatorService _calc;
        private readonly CreditDbContext _context;

        public LoanSimulatorController(CreditDbContext context)
        {
            _calc = new LoanCalculatorService();
            _context = context;
        }

        // POST: api/loansimulate/simulate
        [HttpPost("simulate")]
        public IActionResult SimulateLoan([FromBody] LoanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int durationMonths = Math.Max(1,
                (request.MaturityDate.Year - DateTime.Now.Year) * 12 +
                (request.MaturityDate.Month - DateTime.Now.Month));

            decimal monthlyPayment = _calc.CalculateMonthlyPayment(request.Principal, request.AnnualInterestRate, durationMonths);
            var schedule = _calc.BuildSchedule(request.Principal, request.AnnualInterestRate, durationMonths, DateTime.Now);

            var loan = new Loan
            {
                UserId = request.UserId,
                Amount = request.Principal,
                InterestRate = (decimal)request.AnnualInterestRate,
                DurationMonths = durationMonths,
                StartDate = DateTime.Now,
                MaturityDate = request.MaturityDate,
                RemainingBalance = request.Principal
            };

            _context.Loans.Add(loan);
            _context.SaveChanges();

            return Ok(new
            {
                LoanId = loan.Id,
                MonthlyPayment = monthlyPayment,
                PaymentSchedule = schedule,
                RemainingBalance = loan.RemainingBalance
            });
        }

        // GET: api/loansimulate
        [HttpGet]
        public IActionResult GetAllLoans()
        {
            var loans = _context.Loans
                .Include(l => l.User) 
                .Select(l => new
                {
                    l.Id,
                    l.UserId,
                    UserName = l.User.FullName,
                    l.Amount,
                    l.InterestRate,
                    l.DurationMonths,
                    l.StartDate,
                    l.MaturityDate,
                    l.RemainingBalance
                })
                .ToList();

            return Ok(loans);
        }

        // GET: api/loansimulate/{id}
        [HttpGet("{id}")]
        public IActionResult GetLoanById(int id)
        {
            var loan = _context.Loans
                .Include(l => l.User)
                .Where(l => l.Id == id)
                .Select(l => new
                {
                    l.Id,
                    l.UserId,
                    UserName = l.User.FullName,
                    l.Amount,
                    l.InterestRate,
                    l.DurationMonths,
                    l.StartDate,
                    l.MaturityDate,
                    l.RemainingBalance
                })
                .FirstOrDefault();

            if (loan == null)
                return NotFound();

            return Ok(loan);
        }
    }
}
