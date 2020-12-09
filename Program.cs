// Loan Calculation/Amortization Examples:
// https://teamtreehouse.com/community/loan-payment-formula-in-c
// https://www.coderslexicon.com/amortization-definitive-c-c-java-etc/
// https://stackoverflow.com/questions/342281/generating-an-amortization-schedule

using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace LoanCalculator
{
    class Program
    {
        public struct MonthlyLoanAmortizationValues // why doesn't decaring the struct public make its members public?
        {
            public double monthlyPayment;
            public double monthlyInterest;
            public double monthlyPrinciple;
            public double monthlyBalance;
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Loan Calculator");

            var loanAmortizationSchedule = new List<MonthlyLoanAmortizationValues>();
            string inputString;
            double inputNumeric;
            do 
            {
                Console.Write("Enter Loan Amount: ");
                inputString = Console.ReadLine();
            } while (!ValidateNumericRange(inputString, 74000, 0, 10000000, out inputNumeric));
            var loanAmount = inputNumeric; // 74000;
            do
            {
                Console.Write("Enter Down Payment: ");
                inputString = Console.ReadLine();
            } while (!ValidateNumericRange(inputString, 20, 0, 50, out inputNumeric));
            var downPayment = inputNumeric; // in Percent 20%
            do 
            {
                Console.Write("Enter Interest Rate: ");
                inputString = Console.ReadLine();
            } while (!ValidateNumericRange(inputString, 9.75, 0, 50, out inputNumeric));
            var interest = inputNumeric; //  9.75;
            do
            {
                Console.Write("Enter Number of Years: ");
                inputString = Console.ReadLine();
            } while (!ValidateNumericRange(inputString, 29, 0, 100, out inputNumeric));
            var numberOfYears = inputNumeric; // 29;

            double paymentAmount = CalcLoanAmt(loanAmount, interest, numberOfYears, downPayment);

            //Console.WriteLine("Payment Amount: {0:C2}", paymentAmount); // 2 decimal places!
            //Console.WriteLine(String.Format("Payment Amount: {0:C2}", paymentAmount)); // redundant to use String.Format inside Console.WriteLine
            Console.WriteLine($"Payment Amount: {paymentAmount:C2}"); // swift style

            loanAmortizationSchedule = CalcLoanAmortizationSchedule(loanAmount, interest, paymentAmount, downPayment);

            OutputForConsoleLoanAmortizationSchedule(loanAmortizationSchedule);
            OutputForCSVLoanAmortizationSchedule(loanAmortizationSchedule);


            Console.WriteLine("Goodbye");

            static double CalcLoanAmt(double loanAmount, double interest, double numberOfYears, double downPayment) // make downPayment an optional arg w/default value of 0
            {
                // subtract down payment from load amount
                var loanAmountAsFractionOfLoan = DownPaymentAsFractionOfLoan(downPayment);
                //var loanAmountAsFractionOfLoan = (100 - downPayment) / 100.0; // implicit cast to float - otherwise will truncate value to 0!
                loanAmount *= loanAmountAsFractionOfLoan;
                // rate of interest and number of payments for monthly payments
                var rateOfInterest = interest / (12 * 100); // add to method!
                var numberOfPayments = numberOfYears * 12;  // add to method!
                // loan amount = (interest rate * loan amount) / (1 - (1 + interest rate)^(number of payments * -1))
                return (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));
            }

            static List<MonthlyLoanAmortizationValues> CalcLoanAmortizationSchedule(double loanAmount, double interest, double paymentAmount, double downPayment)
            {
                var loanAmountAsFractionOfLoan = DownPaymentAsFractionOfLoan(downPayment);
                //var loanAmountAsFractionOfLoan = (100 - downPayment) / 100.0; // implicit cast to float - otherwise will truncate value to 0!
                var endingBalance = loanAmount * loanAmountAsFractionOfLoan;
                var monthCount = 1;
                var loanAmorizationScheduleLocal = new List<MonthlyLoanAmortizationValues>();
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
                    var monthlyLoanAmortizationValues = new MonthlyLoanAmortizationValues();
                    if ((newBalance + interestPaid) < payment)
                    {
                        monthlyLoanAmortizationValues.monthlyPayment = newBalance + interestPaid;
                        monthlyLoanAmortizationValues.monthlyInterest = interestPaid;
                        monthlyLoanAmortizationValues.monthlyPrinciple = newBalance - interestPaid;
                        monthlyLoanAmortizationValues.monthlyBalance = 0.0;
                        // loanAmorizationScheduleLocal.Add(monthCount + ". Payment: " + (newBalance + interestPaid).ToString("C") + " Interest: " + interestPaid.ToString("C") + " Principle: " + (newBalance - interestPaid).ToString("C") + " Loan Balance is: $0.00");
                    }
                    else
                    {
                        monthlyLoanAmortizationValues.monthlyPayment = payment;
                        monthlyLoanAmortizationValues.monthlyInterest = interestPaid;
                        monthlyLoanAmortizationValues.monthlyPrinciple = principlePaid;
                        monthlyLoanAmortizationValues.monthlyBalance = endingBalance;
                        // loanAmorizationScheduleLocal.Add(monthCount + ". Payment: " + payment.ToString("C") + " Interest: " + interestPaid.ToString("C") + " Principle: " + principlePaid.ToString("C") + " Loan Balance is: " + endingBalance.ToString("C"));
                    }
                    loanAmorizationScheduleLocal.Add(monthlyLoanAmortizationValues);
                    monthCount++;
                }
                return loanAmorizationScheduleLocal;
            }

            static void OutputForConsoleLoanAmortizationSchedule(List<MonthlyLoanAmortizationValues> loanAmortizationSchedule)
            {
                int ctr = 1;
                foreach (var monthlyLoanAmortizationValues in loanAmortizationSchedule)
                {
                    string line = String.Format("{0,5}. Payment: {1,10:C}  Interest: {2,10:C} Principle: {3,10:C}  Loan Balance: {4,16:C}",ctr++, 
                                                monthlyLoanAmortizationValues.monthlyPayment,
                                                monthlyLoanAmortizationValues.monthlyInterest,
                                                monthlyLoanAmortizationValues.monthlyPrinciple,
                                                monthlyLoanAmortizationValues.monthlyBalance
                                              );
                    Console.WriteLine(line);
                }
            }

            static void OutputForCSVLoanAmortizationSchedule(List<MonthlyLoanAmortizationValues> loanAmortizationSchedule)
            {
                // Write to local file: 
                //   C:\Users\dmlau\Source\Repos\LoanCalculator\bin\Debug\netcoreapp3.1\LoanAmortizationSchedule.csv
                // TODO email file
                // TODO Output month # in amortization schedule additionally as montn/Year from today's date
                string fileNameCSV = "LoanAmortizationSchedule.csv";
                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileNameCSV))
                    {
                        int ctr = 1;
                        string line = "Month,Payment,Interest,Principle,Balance";
                        Console.WriteLine(line);
                        file.WriteLine(line);
                        foreach (var monthlyLoanAmortizationValues in loanAmortizationSchedule)
                        {
                            line = String.Format("{0},\"{1:C}\",\"{2:C}\",\"{3:C}\",\"{4:C}\"", ctr++,
                                                        monthlyLoanAmortizationValues.monthlyPayment,
                                                        monthlyLoanAmortizationValues.monthlyInterest,
                                                        monthlyLoanAmortizationValues.monthlyPrinciple,
                                                        monthlyLoanAmortizationValues.monthlyBalance
                                                      );
                            Console.WriteLine(line);
                            file.WriteLine(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in writing CSV file: {0}", ex.ToString());
                }
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    mail.From = new MailAddress("dmlaub123@gmail.com");
                    mail.To.Add("dmlaub123@gmail.com");
                    mail.Subject = "Loan Amortization Schedule";
                    mail.Body = "Loan Amortization Schedule";
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(fileNameCSV);
                    mail.Attachments.Add(attachment);
                    SmtpServer.Port = 587;

                    Console.WriteLine("Domain: {0} UserName: {1} Password: {2}", 
                                                     new System.Net.NetworkCredential().Domain,
                                                     new System.Net.NetworkCredential().UserName,
                                                     new System.Net.NetworkCredential().Password);

                    SmtpServer.Credentials = new System.Net.NetworkCredential(); // "username", "password");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in mailing CSV file: {0}", ex.ToString());
                }
            }

            static bool ValidateNumericRange(string input, double defaultVal, double low, double high, out double result)
            {
                bool ok;
                if (String.IsNullOrWhiteSpace(input))
                {
                    result = defaultVal;
                    ok = true;
                }
                else
                {
                    result = -1;
                    ok = Double.TryParse(input.Trim(), out result);
                    ok = ok && (result > low) && (result < high);
                }
                return ok;
            }

            static double DownPaymentAsFractionOfLoan(double downPayment)
            {
                return (100 - downPayment) / 100.0; // implicit cast to float - otherwise will truncate value to 0!

            }
        }
    }
}
