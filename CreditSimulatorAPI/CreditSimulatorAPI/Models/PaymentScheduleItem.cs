using System;

namespace CreditSimulatorAPI.Models
{
    public class PaymentScheduleItem
    {
        public int MonthNumber { get; set; }
        public decimal PrincipalPayment { get; set; }
        public decimal InterestPayment { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal RemainingBalance { get; set; }
    }
}
