namespace CreditSimulatorAPI.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; } // % annual
        public int DurationMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime MaturityDate { get; set; }

        public decimal RemainingBalance { get; set; }

        public ICollection<Payment> Payments { get; set; }
    }
}
