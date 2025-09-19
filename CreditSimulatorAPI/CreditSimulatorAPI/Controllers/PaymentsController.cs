using CreditSimulatorAPI.Data;
using CreditSimulatorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreditSimulatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly CreditDbContext _context;

        public PaymentsController(CreditDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPayment(int loanId, decimal amount)
        {
            var loan = await _context.Loans.Include(l => l.Payments).FirstOrDefaultAsync(l => l.Id == loanId);
            if (loan == null) return NotFound("Loan not found.");

            if (loan.RemainingBalance <= 0) return BadRequest("Loan already fully paid.");

            decimal interestPortion = loan.RemainingBalance * (loan.InterestRate / 100 / 12);
            decimal principalPortion = amount - interestPortion;
            if (principalPortion < 0) principalPortion = 0;

            loan.RemainingBalance -= principalPortion;
            if (loan.RemainingBalance < 0) loan.RemainingBalance = 0;

            var payment = new Payment
            {
                LoanId = loanId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                InterestPortion = interestPortion,
                PrincipalPortion = principalPortion
            };

            _context.Payments.Add(payment);
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment registered successfully", balance = loan.RemainingBalance });
        }

        [HttpGet("balance/{loanId}")]
        public async Task<IActionResult> GetBalance(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return NotFound("Loan not found.");

            return Ok(new { balance = loan.RemainingBalance });
        }
    }
}
