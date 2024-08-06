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
        // Matches if there a date at the start of line
        [GeneratedRegex(@"^[a-z]{3}\d{2}e?\s",
            RegexOptions.IgnoreCase)]
        public static partial Regex IsNewEntryGR();

        /** Extracts important data from first line of entries into 4 capture groups
         * Regex breakdown from left to right:
         * date posted (capture group 1)
         * sometimes E is appended to the date, bank statement doesn't clarify
         * medium of transaction (capture group 2)
         * amount transacted (capture group 3)
         * amount sign (capture group 4)
         * new balance amount
         */
        [GeneratedRegex(@"(^[a-z]{3}\d{2})e?\s(.+)\s([\d{1-3},]*\d+\.\d{2})(-?)\s[\d{1-3},]*\d+\.\d{2}$",
            RegexOptions.IgnoreCase)]
        public static partial Regex firstLineGR();

        /** Extracts the organization from debit entries while trimming as much as i can muster
         * Regex breakdown from left to right:
         * debit transaction number
         * square tag (optional non-capture group)
         * toast tag (optional non-capture group)
         * organization (lazy capture group 1) <- this is the important part
         * amazon order number (optional non-capture group)
         * store number / address (optional non-capture group)
         * date of purchase (non-capture group)
         */
        [GeneratedRegex(@"^\w+\s(?:SQ\s\*)?(?:TST\*\s)?(.+?)(?:\*\w+\s.+)?(?:\s[#\w]?\d+.*)?(?:\s\d\d-\d\d-\d\d)$")]
        public static partial Regex DebitOrganizationGR();

        /** Matches each potential org to a group to check success for decisions
         * H-E-B (capture group 1)
         * TSU (capture group 2)
         */
        [GeneratedRegex(@"(H-E-B)|(TSU)")]
        public static partial Regex EftOrganization_GR();


        public enum Account
        {
            Checking,
            Savings,
            Out
        }

        public enum Category
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
        private readonly Category category = Category.Unknown;

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
                    this.organziation = DebitOrganizationGR().Match(secondLine).Groups[1].Value;
                    this.category = GetCatFromOrg(this.organziation);
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
                    if (EftOrganization_GR().Match(medium).Groups[1].Success)//HEB
                    {
                        this.description = "Paycheck";
                        this.organziation = "HEB";
                        this.category = Category.Income_HEB;
                    } 
                    else if (EftOrganization_GR().Match(medium).Groups[2].Success)//TSU
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
        public string ToCSVLine()
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

        private static readonly Dictionary<string, Category> orgToCatDict = FileProcessor.GetSavedOrgs();


        /// <summary>
        /// Gets the category for a given org
        /// adds it to the dictionary if it doesn't already exist
        /// then saves the dictionary
        /// </summary>
        /// <param name="org">org to get category for</param>
        /// <returns>Corresponding category for an org</returns>
        static Category GetCatFromOrg(string org)
        {
            
            if (!orgToCatDict.TryGetValue(org, out Category cat))
                orgToCatDict.Add(org, Category.Unknown);

            FileProcessor.SaveOrgsAndCats(orgToCatDict);

            return cat;
        }

    }
}
