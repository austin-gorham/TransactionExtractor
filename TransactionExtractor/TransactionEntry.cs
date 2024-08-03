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
        [GeneratedRegex(@"\sEFT\s")]
        public static partial Regex IsSingleLineGeneratedRegex();

        [GeneratedRegex(@"(^[a-z]{3}\d{2})e?\s(.+)\s([\d{1-3},]*\d+\.\d{2})(-?)\s[\d{1-3},]*\d+\.\d{2}$")]
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

        public TransactionEntry(string date, Account to, Account from, string amt, string description, Category cat) =>
            (this.date, this.accountTo, this.accountFrom, this.amount, this.description, this.category) = (date, to, from, amt, description, cat);

        public TransactionEntry(string firstLine, string secondLine = "" )
        {
            Match firstLineMatch = firstLineGR().Match(firstLine);

            this.date = firstLineMatch.Groups[1].Value;
            string medium = firstLineMatch.Groups[2].Value;
            this.amount = firstLineMatch.Groups[3].Value;
            string sign = firstLineMatch.Groups[4].Value;

            if (secondLine != string.Empty)
            {
                //do description things
            }

            switch( medium.Substring(0, 3) ) { 
                case "Deb" :
                    this.accountTo = Account.Out;
                    this.accountFrom = Account.Checking;
                    break;
                case "EFT":
                    this.accountTo = Account.Out;
                    this.accountFrom = Account.Checking;
                    break;
                case "Div":
                    this.accountTo = Account.Checking;
                    this.accountFrom = Account.Out;
                    break;
                case "Tra":
                    this.accountTo = Account.Out;
                    this.accountFrom = Account.Checking;
                    break;
                default:
                    break;
                    
            }


            this.description = "";


        }

        public string ToCSVLine()
        {

            StringBuilder sb = new StringBuilder();

            return sb.ToString();
        }

    }
}
