using CreditSimulatorAPI.Models;

namespace CreditSimulatorAPI.Services
{
    public class LoanCalculatorService
    {
        public decimal CalculateMonthlyPayment(decimal principal, double annualRate, int termMonths)
        {
            if (termMonths <= 0) return 0;
            double monthlyRate = annualRate / 100.0 / 12.0;

            if (monthlyRate == 0)
                return Math.Round(principal / termMonths, 2);

            double p = (double)principal;
            double pow = Math.Pow(1 + monthlyRate, termMonths);
            double monthly = p * monthlyRate * pow / (pow - 1);

            return Math.Round((decimal)monthly, 2);
        }

        public List<PaymentScheduleItem> BuildSchedule(decimal principal, double annualRate, int termMonths, DateTime startDate)
        {
            var schedule = new List<PaymentScheduleItem>();
            decimal monthlyPayment = CalculateMonthlyPayment(principal, annualRate, termMonths);
            decimal remaining = principal;
            double monthlyRate = annualRate / 100.0 / 12.0;

            for (int m = 1; m <= termMonths; m++)
            {
                decimal interest = Math.Round(remaining * (decimal)monthlyRate, 2);
                decimal principalPortion = monthlyPayment - interest;
                if (m == termMonths) // korrigjon rrumbullakosjen
                {
                    principalPortion = remaining;
                    monthlyPayment = principalPortion + interest;
                }
                remaining = Math.Round(remaining - principalPortion, 2);

                schedule.Add(new PaymentScheduleItem
                {
                    MonthNumber = m,
                    DueDate = startDate.AddMonths(m),
                    PrincipalPayment = principalPortion,
                    InterestPayment = interest,
                    TotalPayment = monthlyPayment,
                    RemainingBalance = Math.Max(remaining, 0)
                });
            }
            return schedule;
        }
    }
}
