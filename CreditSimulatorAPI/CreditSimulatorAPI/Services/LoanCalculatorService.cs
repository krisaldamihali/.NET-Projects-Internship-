using CreditSimulatorAPI.Models;
using System;
using System.Collections.Generic;

namespace CreditSimulatorAPI.Services
{
    public class LoanCalculatorService
    {
        public List<PaymentScheduleItem> GeneratePaymentSchedule(LoanRequest loan)
        {
            var schedule = new List<PaymentScheduleItem>();

            int totalMonths = ((loan.MaturityDate.Year - DateTime.Now.Year) * 12) + loan.MaturityDate.Month - DateTime.Now.Month;
            if (totalMonths <= 0) totalMonths = 1;

            decimal monthlyInterestRate = (decimal)(loan.AnnualInterestRate / 100 / 12);
            decimal monthlyPayment = loan.Principal * monthlyInterestRate / (1 - (decimal)Math.Pow((double)(1 + monthlyInterestRate), -totalMonths));

            decimal remainingBalance = loan.Principal;

            for (int month = 1; month <= totalMonths; month++)
            {
                decimal interestPayment = remainingBalance * monthlyInterestRate;
                decimal principalPayment = monthlyPayment - interestPayment;
                remainingBalance -= principalPayment;

                schedule.Add(new PaymentScheduleItem
                {
                    MonthNumber = month,
                    PrincipalPayment = Math.Round(principalPayment, 2),
                    InterestPayment = Math.Round(interestPayment, 2),
                    TotalPayment = Math.Round(monthlyPayment, 2),
                    RemainingBalance = Math.Round(Math.Max(remainingBalance, 0), 2)
                });
            }

            return schedule;
        }

        public decimal CalculateMonthlyPayment(LoanRequest loan)
        {
            int totalMonths = ((loan.MaturityDate.Year - DateTime.Now.Year) * 12) + loan.MaturityDate.Month - DateTime.Now.Month;
            if (totalMonths <= 0) totalMonths = 1;

            decimal monthlyInterestRate = (decimal)(loan.AnnualInterestRate / 100 / 12);
            decimal monthlyPayment = loan.Principal * monthlyInterestRate / (1 - (decimal)Math.Pow((double)(1 + monthlyInterestRate), -totalMonths));
            return Math.Round(monthlyPayment, 2);
        }
    }
}
