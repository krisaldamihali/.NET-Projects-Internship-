namespace CreditSimulatorAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public Loan Loan { get; set; }

        public decimal Amount { get; set; }
        public decimal PrincipalPortion { get; set; }
        public decimal InterestPortion { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
