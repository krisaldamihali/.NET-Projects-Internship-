using System;
using System.ComponentModel.DataAnnotations;

namespace CreditSimulatorAPI.Models
{
    public class LoanRequest
    {
        [Required]
        public decimal Principal { get; set; }      // Loan amount

        [Required]
        public DateTime MaturityDate { get; set; } // Loan maturity date

        [Required]
        public double AnnualInterestRate { get; set; } // Annual interest rate in %
    }
}
