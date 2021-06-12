using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace geektrust
{
    class The_Ledger_Co
    {
        public void Run(string[] args)
        {
            //string input = File.ReadAllText(args[0]);

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string file = Path.Combine(path, "Input1.txt"); // copy the file from TheLedgerCo\Input folder to bin
            string input = File.ReadAllText(file);

            string[] lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            List<Loan> lstLoan = new List<Loan>();
            List<Payment> lstPayment = new List<Payment>();

            foreach (string line in lines)
            {
                List<string> lstInput = line.Split(' ').ToList();

                switch (lstInput[0])
                {
                    case Commands.LOAN:

                        lstLoan.Add(new Loan(lstInput[1], lstInput[2], Convert.ToInt64(lstInput[3]), Convert.ToInt32(lstInput[4]), Convert.ToInt32(lstInput[5])));

                        break;

                    case Commands.PAYMENT:

                        lstPayment.Add(new Payment(lstInput[1], lstInput[2], Convert.ToInt64(lstInput[3]), Convert.ToInt32(lstInput[4])));

                        break;

                    case Commands.BALANCE:

                        string BANK_NAME = lstInput[1];
                        string BORROWER_NAME = lstInput[2];
                        int EMI_NO = Convert.ToInt32(lstInput[3]);

                        Loan loan = lstLoan.Where(l => l.BANK_NAME.Equals(BANK_NAME) && l.BORROWER_NAME.Equals(BORROWER_NAME)).FirstOrDefault();

                        float AMOUNT_REPAY = loan.GetAmountToBePaidToBank();

                        int NO_OF_EMIS = loan.NO_OF_YEARS * 12;
                        int AMOUNT_PER_EMI = Convert.ToInt32(Math.Ceiling(AMOUNT_REPAY / NO_OF_EMIS));

                        List<Payment> payments = lstPayment.Where(l => l.BANK_NAME.Equals(BANK_NAME) && l.BORROWER_NAME.Equals(BORROWER_NAME) && l.EMI_NO <= EMI_NO).ToList();

                        long LUMP_SUM_AMOUNT_PAID = payments.Sum(p => p.LUMP_SUM_AMOUNT);
                        long TOTAL_AMOUNT_PAID = LUMP_SUM_AMOUNT_PAID + (AMOUNT_PER_EMI * EMI_NO);

                        if (TOTAL_AMOUNT_PAID > AMOUNT_REPAY)   // manages the case if last EMI is more than the remaining amount to pay
                        {
                            TOTAL_AMOUNT_PAID = Convert.ToInt64(Math.Ceiling(AMOUNT_REPAY));
                        }

                        float AMOUNT_DUE = AMOUNT_REPAY - TOTAL_AMOUNT_PAID;
                        int NO_OF_EMIS_LEFT = Convert.ToInt32(Math.Ceiling(AMOUNT_DUE / AMOUNT_PER_EMI));

                        Console.Out.WriteLine(string.Format("{0} {1} {2} {3}", BANK_NAME, BORROWER_NAME, TOTAL_AMOUNT_PAID, NO_OF_EMIS_LEFT));

                        break;

                    default:
                        break;
                }
            }
        }
    }
    static class Commands
    {
        public const string LOAN = "LOAN";
        public const string PAYMENT = "PAYMENT";
        public const string BALANCE = "BALANCE";
    }

    class Loan
    {
        public Loan(string BANK_NAME, string BORROWER_NAME, long PRINCIPAL, int NO_OF_YEARS, int RATE_OF_INTEREST)
        {
            this.BANK_NAME = BANK_NAME;
            this.BORROWER_NAME = BORROWER_NAME;
            this.PRINCIPAL = PRINCIPAL;
            this.NO_OF_YEARS = NO_OF_YEARS;
            this.RATE_OF_INTEREST = RATE_OF_INTEREST;
        }

        /// <summary>
        /// calculates the amount to be paid, to the bank
        /// </summary>
        /// <returns>AMOUNT_REPAY</returns>
        public float GetAmountToBePaidToBank()
        {
            long INTEREST = (PRINCIPAL * NO_OF_YEARS * RATE_OF_INTEREST) / 100;
            float AMOUNT_REPAY = PRINCIPAL + INTEREST;

            return AMOUNT_REPAY;
        }

        public string BANK_NAME;
        public string BORROWER_NAME;
        public long PRINCIPAL;
        public int NO_OF_YEARS;
        public int RATE_OF_INTEREST;
    }

    class Payment
    {
        public Payment(string BANK_NAME, string BORROWER_NAME, long LUMP_SUM_AMOUNT, int EMI_NO)
        {
            this.BANK_NAME = BANK_NAME;
            this.BORROWER_NAME = BORROWER_NAME;
            this.LUMP_SUM_AMOUNT = LUMP_SUM_AMOUNT;
            this.EMI_NO = EMI_NO;
        }

        public string BANK_NAME;
        public string BORROWER_NAME;
        public long LUMP_SUM_AMOUNT;
        public int EMI_NO;
    }
}
