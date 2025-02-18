using System.Text.RegularExpressions;

namespace TransactionExtractor
{
    internal static partial class RegexContainer
    {
        [GeneratedRegex(@"(\w+)(\s.*)?", RegexOptions.IgnoreCase)]
        internal static partial Regex CommandGR();


        //Extracts the file extension for a given file path
        [GeneratedRegex(@"(\.\w*?)$")]
        internal static partial Regex FileExtensionGR();

        /**Indicator for entering data block during parsing
         * lines following this will be transaction entries
         * Contains matches for the two potential lines preceding the datablock
         **/
        [GeneratedRegex(@"(?:^DIRECT CHOICE ACCT# 2 .* PREVIOUS BALANCE .*\d\d$)|(?:^CHARGE$)")]
        internal static partial Regex IsDataBlockEntranceGR();


        /**Indicator for exit data block during parsing
         * this line and lines after will be superfluous
         * Contains matches for the three potential lines proceeding after the datablock
         **/
        [GeneratedRegex(@"(?:^Continued on page \d+$)|(?:^Notice)|(?:^\*\*\*)")]
        internal static partial Regex IsDataBlockExitGR();


        // Matches if there a date at the start of line
        [GeneratedRegex(@"^[a-z]{3}\d{2}e?\s",
            RegexOptions.IgnoreCase)]
        internal static partial Regex IsNewEntryGR();

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
        internal static partial Regex FirstLineExtractorGR();

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
        internal static partial Regex DebitOrgExtractorGR();

        /** Matches each potential org to a group to check success for decisions
         * H-E-B (capture group 1)
         * TSU (capture group 2)
         */
        [GeneratedRegex(@"(H-E-B)|(TSU)")]
        internal static partial Regex WhichEftOrgGR();
    }
}
