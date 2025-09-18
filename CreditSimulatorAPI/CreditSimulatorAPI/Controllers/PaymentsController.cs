using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreditSimulatorAPI.Data;
using CreditSimulatorAPI.Models;

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

        // Regjistrimi i pagesës
        [HttpPost("register")]
        public async Task<IActionResult> RegisterPayment(int loanId, decimal amount)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return NotFound("Loan not found.");

            var payment = new Payment
            {
                LoanId = loanId,
                Amount = amount,
                PaymentDate = DateTime.Now
            };

            loan.RemainingBalance -= amount;

            _context.Payments.Add(payment);
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment registered successfully", balance = loan.RemainingBalance });
        }

        // Shfaqja e balances së mbetur
        [HttpGet("balance/{loanId}")]
        public async Task<IActionResult> GetBalance(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null) return NotFound("Loan not found.");

            return Ok(new { balance = loan.RemainingBalance });
        }
    }
}
