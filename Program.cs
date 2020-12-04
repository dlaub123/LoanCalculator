using System;
using System.Collections.Generic;

namespace LoanCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Loan Calculator");

            var loanAmortizationSchedule = new List<String>();

            var downPayment = 20; // in Percent
            var loanAmount = 74000;
            var interest = 9.75;
            var numberOfYears = 29;

            double paymentAmount = CalcLoanAmt(loanAmount, interest, numberOfYears, downPayment);

            //Console.WriteLine("Payment Amount: {0:C2}", paymentAmount); // 2 decimal places!
            //Console.WriteLine(String.Format("Payment Amount: {0:C2}", paymentAmount)); // redundant to use String.Format inside Console.WriteLine
            Console.WriteLine($"Payment Amount: {paymentAmount:C2}"); // swift style

            loanAmortizationSchedule = CalcLoanAmortizationSchedule(loanAmount, interest, paymentAmount);

            Console.WriteLine("Goodbye");

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

            static List<String> CalcLoanAmortizationSchedule(int loanAmount, double interest, double paymentAmount)
            {
                var endingBalance = loanAmount * 0.8;
                var monthCount = 1;
                var loanAmorizationScheduleLocal = new List<String>();
                while (endingBalance > 0.0)
                {
                    // Simple Refactoring:
                    //   eliminate duplicate variables
                    //   rename variables with camelCasing vs. _ casing
                    // Advanced Refactoring:
                    //   store output in struct based List w/o output formating to facilitate 3 final outputs: a) console writes, b) bound data grid, c) csv file
                    var newBalance = endingBalance;
                    var annualRate = interest;
                    var payment = paymentAmount;
                    // Calculate interest by multiplying rate against balance
                    var interestPaid = newBalance * (annualRate / 100.0 / 12.0);
                    // Subtract interest from your payment
                    var principlePaid = payment - interestPaid;
                    // Subtract final payment from running balance
                    endingBalance = newBalance - principlePaid;
                    // If the balance remaining plus its interest is less than payment amount
                    // Then print out 0 balance, the interest paid and that balance minus the interest will tell us
                    // how much principle you paid to get to zero.

                    // Refactor to List of struct vs string - i.e. store each element (e.g. interest & principle) as is w/o formatting
                    // Then display w/formatting in console writes or bind to visual grid or write to CSV file w/header
                    if ((newBalance + interestPaid) < payment)
                    {
                        loanAmorizationScheduleLocal.Add(monthCount + ". Payment: " + (newBalance + interestPaid).ToString("C") + " Interest: " + interestPaid.ToString("C") + " Principle: " + (newBalance - interestPaid).ToString("C") + " Loan Balance is: $0.00");
                    }
                    else
                    {
                        loanAmorizationScheduleLocal.Add(monthCount + ". Payment: " + payment.ToString("C") + " Interest: " + interestPaid.ToString("C") + " Principle: " + principlePaid.ToString("C") + " Loan Balance is: " + endingBalance.ToString("C"));
                    }
                    monthCount++;
                }
                return loanAmorizationScheduleLocal;
            }
        }
    }
}
