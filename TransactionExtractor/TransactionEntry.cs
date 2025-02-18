using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransactionExtractor
{
    internal class TransactionEntry
    {

        internal enum Account
        {
            Checking,
            Savings,
            Out
        }

        internal enum Category
        {
            Unknown,
            Transfer,
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

        private readonly string date;
        private readonly Account accountTo;
        private readonly Account accountFrom;
        private readonly string amount;
        private readonly string description = "-";
        private readonly string organziation = "-";
        private Category category = Category.Unknown;//mutable since might need update after construction

        /// <summary>
        /// constructs an entry object from text records
        /// </summary>
        /// <param name="firstLine">primary part of entry; includes date, medium, amount, sign, as well as some extraneous data which is ignored</param>
        /// <param name="secondLine">optional secondary part of entry which includes some descriptive info, sometimes useful, sometimes not</param>
        internal TransactionEntry(string firstLine, string secondLine = "" )
        {
            //Captures desired data into groups via regex
            Match firstLineMatch = RegexContainer.FirstLineExtractorGR().Match(firstLine);

            //assigns data, some is fine as is, some is needs further processing
            this.date = firstLineMatch.Groups[1].Value;
            string medium = firstLineMatch.Groups[2].Value;
            this.amount = firstLineMatch.Groups[3].Value.Replace(",","");
            string sign = firstLineMatch.Groups[4].Value;

            //shortened medium string for bulk of comparisons
            string medShort = medium[..3];

            //Console.WriteLine("New entry for: " + this.date);

            
            //Default non-checking account, possible modified in data decision tree
            Account nonChecking = Account.Out;

            //Data decision tree
            switch (medShort)
            {
                case "DEB":
                    this.description = "-";
                    this.organziation = RegexContainer.DebitOrgExtractorGR().Match(secondLine).Groups[1].Value;
                    this.category = OrgToCatDictionary.GetCatFromOrgAndUpdate(this.organziation);
                    break;
                case "TRA":
                    nonChecking = Account.Savings;//Transfers move to/from savings instead of out
                    this.description = "Transfer";
                    this.organziation = "-";
                    this.category = Category.Transfer;
                    break;
                case "DIV":
                    this.description = "Dividend";
                    this.organziation = "Educators";
                    this.category = Category.Income_Interest;
                    break;
                case "EFT":
                    if (RegexContainer.WhichEftOrgGR().Match(medium).Groups[1].Success)//HEB
                    {
                        this.description = "Paycheck";
                        this.organziation = "HEB";
                        this.category = Category.Income_HEB;
                    } 
                    else if (RegexContainer.WhichEftOrgGR().Match(medium).Groups[2].Success)//TSU
                    {
                        this.description = "Tuition";
                        this.organziation = "Tarleton State University";
                        this.category = Category.Education_School;
                    }
                    break;
                default: 
                    break;
            }

            //Sign determines which account is gaining vs losing
            if (sign == "-")
            {
                this.accountTo = nonChecking;
                this.accountFrom = Account.Checking;
            }
            else
            {
                this.accountTo = Account.Checking;
                this.accountFrom = nonChecking;
            }

        }

        /// <summary>
        /// Builds the csv line for a given entry
        /// essentially a tostring but more specific
        /// </summary>
        /// <returns>a line in CSV format representing the transaction entry</returns>
        internal string ToCSVLine()
        {
            StringBuilder sb = new();

            sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6}\n", 
                this.date,
                this.amount,
                this.description,
                this.organziation,
                this.category.ToString().Replace("_"," - "),
                this.accountTo,
                this.accountFrom);

            return sb.ToString();
        }




        /// <summary>
        /// Updates the OrgToCatDictionary
        /// Tries to update category if unknown
        /// assuming dictionary and corresponding
        /// cat has changed since construction
        /// </summary>
        internal void UpdateUnknownCat()
        {
            OrgToCatDictionary.UpdateDictionary();
            if (this.category == Category.Unknown)
            {
                this.category = OrgToCatDictionary.GetCatFromOrg(this.organziation);
                if (this.category != Category.Unknown) 
                    Console.WriteLine("> Entry updated: " + this.ToCSVLine());
            }
        }

    }
}
