using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransactionExtractor
{
    internal partial class TransactionEntry
    {
        [GeneratedRegex(@"^[a-z]{3}\d{2}e?\s",
            RegexOptions.IgnoreCase)]
        public static partial Regex IsNewEntryGR();

        [GeneratedRegex(@"(^[a-z]{3}\d{2})e?\s(.+)\s([\d{1-3},]*\d+\.\d{2})(-?)\s[\d{1-3},]*\d+\.\d{2}$",
            RegexOptions.IgnoreCase)]
        public static partial Regex firstLineGR();


        public enum Account
        {
            Checking,
            Savings,
            Out
        }

        public enum Category
        {
            Transportation_Fuel,
            Transportation_Miscellaneous,
            Food_Internal,
            Food_External,
            Pet_Supplies,
            Pet_Medical,
            Health_Doctor,
            Health_Drug,
            Education_School,
            Education_Career,
            Education_Political,
            Education_Personal,
            Giving_Charity,
            Giving_Political,
            Personal_Fitness,
            Personal_Hygeine,
            Personal_Apparel,
            Personal_Appearance,
            Personal_Utility,
            Personal_Miscellaneous,
            Recreation_Alcohol,
            Recreation_Media,
            Recreation_Miscellaneous,
            Savings_Emergency,
            Savings_School,
            Income_HEB,
            Income_Interest
        }

        private string date;
        private Account accountTo;
        private Account accountFrom;
        private string amount;
        private string description;
        private Category category;

        /// <summary>
        /// constructs an entry object from text records
        /// </summary>
        /// <param name="firstLine">primary part of entry; includes date, medium, amount, sign, as well as some extraneous data which is ignored</param>
        /// <param name="secondLine">optional secondary part of entry which includes some descriptive info, sometimes useful, sometimes not</param>
        public TransactionEntry(string firstLine, string secondLine = "" )
        {
            //Captures desired data into groups via regex
            Match firstLineMatch = firstLineGR().Match(firstLine);

            //assigns data, some is fine as is, some is needs further processing
            this.date = firstLineMatch.Groups[1].Value;
            string medium = firstLineMatch.Groups[2].Value;
            this.amount = firstLineMatch.Groups[3].Value.Replace(",","");
            string sign = firstLineMatch.Groups[4].Value;

            //Console.WriteLine("New entry for: " + this.date);

            if (secondLine != string.Empty)
            {
                //TODO: description things
            }

            //TRANSFERs are the medium that won't have an Account.Out in them
            Account nonChecking = medium[..3] == "TRA" ? Account.Savings : Account.Out;

            //Sign determines which account is gaining vs losing
            if( sign == "-") 
            {
                this.accountTo = nonChecking;
                this.accountFrom = Account.Checking;
            } else 
            {
                this.accountTo = Account.Checking;
                this.accountFrom = nonChecking;
            }


            this.description = secondLine;


        }

        /// <summary>
        /// Builds the csv line for a given entry
        /// essentially a tostring but more specific
        /// </summary>
        /// <returns>a line in CSV format representing the transaction entry</returns>
        public string ToCSVLine()
        {
            StringBuilder sb = new();

            sb.AppendFormat("{0},{1},-,{2},{3},{4},{5}\n", 
                this.date,
                this.amount,
                this.description,
                this.category,
                this.accountTo,
                this.accountFrom);

            return sb.ToString();
        }

    }
}
