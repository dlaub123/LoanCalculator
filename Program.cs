using System;

namespace LoanCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Loan Calculator");

            var downPayment = 20; // in Percent
            var loanAmount = 74000;
            var interest = 9.75;
            var numberOfYears = 29;

            double paymentAmount = CalcLoanAmt(loanAmount, interest, numberOfYears, downPayment);

            Console.WriteLine(paymentAmount); // 2 decimal places!

            static double CalcLoanAmt(double loanAmount, double interest, int numberOfYears, int downPayment) // make downPayment an optional arg w/default value of 0
            {
                // subtract down payment from load amount
                var downPaymentAmount = (100 - downPayment) / 100.0; // implicit cast to float - otherwise will truncate value to 0!
                loanAmount *= downPaymentAmount;
                // rate of interest and number of payments for monthly payments
                var rateOfInterest = interest / (12 * 100); // add to method!
                var numberOfPayments = numberOfYears * 12;  // add to method!
                // loan amount = (interest rate * loan amount) / (1 - (1 + interest rate)^(number of payments * -1))
                return (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));
            }
        }
    }
}
