using System.ComponentModel.DataAnnotations;

namespace CreditSimulatorAPI.Models
{
    public class LoanRequest
    {
        [Required]
        public decimal Principal { get; set; }

        [Required]
        public DateTime MaturityDate { get; set; }

        [Required]
        public double AnnualInterestRate { get; set; }

        [Required]
        public int UserId { get; set; } // i lidhur me tabelën User
    }
}
