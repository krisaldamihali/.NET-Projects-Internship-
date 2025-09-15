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

        public LoanSimulatorController()
        {
            _calculator = new LoanCalculatorService();
        }

        [HttpPost("simulate")]
        public IActionResult SimulateLoan([FromBody] LoanRequest loan)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            decimal monthlyPayment = _calculator.CalculateMonthlyPayment(loan);
            List<PaymentScheduleItem> schedule = _calculator.GeneratePaymentSchedule(loan);

            return Ok(new
            {
                MonthlyPayment = monthlyPayment,
                PaymentSchedule = schedule
            });
        }
    }
}
