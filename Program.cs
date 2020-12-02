using System;

namespace LoanCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Loan Calculator");
            var loanAmount = 74000 * 0.8;
            var interest = 9.75;
            var numberOfYears = 29;

            // rate of interest and number of payments for monthly payments
            var rateOfInterest = interest / (12 * 100);
            var numberOfPayments = numberOfYears * 12;

            double paymentAmount = CalcLoanAmt(loanAmount, rateOfInterest, numberOfPayments);

            Console.WriteLine(paymentAmount); // 2 decimal places!

            static double CalcLoanAmt(double loanAmount, double rateOfInterest, int numberOfPayments)
            {
                // loan amount = (interest rate * loan amount) / (1 - (1 + interest rate)^(number of payments * -1))
                return (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));
            }
        }
    }
}
