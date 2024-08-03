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

        public string date;
        public Account accountTo;
        public Account accountFrom;
        public string amount;
        public string description;
        public Category category;

        public TransactionEntry(string date, Account to, Account from, string amt, string description, Category cat) =>
            (this.date, this.accountTo, this.accountFrom, this.amount, this.description, this.category) = (date, to, from, amt, description, cat);

        public TransactionEntry(string firstLine, string secondLine = "" )
        {

        }
        
        public string ToCSVLine()
        {

            StringBuilder sb = new StringBuilder();

            return sb.ToString();
        }

    }
}
