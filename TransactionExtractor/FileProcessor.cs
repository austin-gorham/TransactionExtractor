using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static TransactionExtractor.TransactionEntry;

namespace TransactionExtractor
{
    internal static partial class FileProcessor
    {

        [GeneratedRegex(@"(\.\w{1,3})$")]
        public static partial Regex fileTypeGR();


        const string orgFile = "OrgsAndCats.txt";


        /// <summary>
        /// Gets entries from a file path and constructs them into a list of entry objects
        /// </summary>
        /// <param name="file">file to extract from</param>
        /// <returns>list of transaction entry objects</returns>
        public static List<TransactionEntry> GetEntries(string file)
        {

            List<TransactionEntry> list = [];

            using StreamReader sr = new(
                new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read));

            /*
                * Entries can be 1 or 2 lines
                * so need to queue up two at a time
                * and check if they are both entries
                * before creating an entry out of the first line
                * tags the second line along if it is not an entry (assumed part of first)
                */
            string newEntry = "";
            while (sr.Peek() != -1)
            {
                //Console.WriteLine("reading new line");
                string nextLine = sr.ReadLine()?.Trim() ?? "";
                if ( TransactionEntry.IsNewEntryGR().IsMatch(nextLine) )
                {
                    if (!String.IsNullOrEmpty(newEntry))
                        list.Add(new TransactionEntry(newEntry));
                    newEntry = nextLine;
                } else if (!String.IsNullOrEmpty(newEntry)) 
                { 
                    list.Add(new TransactionEntry(newEntry, nextLine));
                    newEntry = String.Empty;
                }
            }
            if (!String.IsNullOrEmpty(newEntry))
                list.Add(new TransactionEntry(newEntry));

            return list;
        }

        /// <summary>
        /// Builds the CSV file given a list of transaction entries
        /// file name will be output.csv
        /// will overwrite existing file of said name at provided path
        /// </summary>
        /// <param name="file">output file path location; ignores specified file, file name is output.csv</param>
        /// <param name="entries">list of entries to populate file with</param>
        public static void WriteEntries(string file, List<TransactionEntry> entries)
        {

            string outputFile;

            if (fileTypeGR().Match(file.TrimEnd()).Success)
                outputFile = fileTypeGR().Replace(file, ".csv");
            else
                outputFile = "output.csv";


            using StreamWriter sw = new(
                new FileStream(outputFile, FileMode.Create, FileAccess.Write));

            foreach (var entry in entries)
            {
                sw.Write(entry.ToCSVLine());
                    
            }
            
            Console.WriteLine("CSV file output: " + outputFile);
        }

        /// <summary>
        /// Loads the orgs and cats from file and fills a dictionary
        /// if a category doesn't parse, writes to console and sets it to default (Category.Unknown)
        /// </summary>
        /// <returns>dictionary of orgs and corresponding categories</returns>
        public static Dictionary<string, TransactionEntry.Category> GetSavedOrgs()
        {
            Dictionary<string, TransactionEntry.Category> orgsAndCats = [];

            using StreamReader sr = new(
               new FileStream(orgFile, FileMode.OpenOrCreate, FileAccess.Read));

            while (sr.Peek() != -1)
            {
                string record = sr.ReadLine() ?? "";
                string[] fields = record.Split(',');

                if (fields.Length == 2)
                {
                    try
                    {
                        TransactionEntry.Category cat =
                            Enum.Parse<TransactionEntry.Category>(fields[1]);

                        orgsAndCats.Add(fields[0], cat);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Category parse failed: " + fields[1] + "; exception: " + ex.ToString());
                        orgsAndCats.Add(fields[0], 0);
                    }
                }
            }

            return orgsAndCats;
        }

        /// <summary>
        /// Saves the orgsAndCats dictionary
        /// format:
        /// [org],[cat]\n
        /// </summary>
        /// <param name="dict">dictionary to save</param>
        public static void SaveOrgsAndCats(Dictionary<string, TransactionEntry.Category> dict)
        {
            using StreamWriter sw = new(
                new FileStream(orgFile, FileMode.Create, FileAccess.Write));

            foreach (var kvp in dict)
            {
                sw.WriteLine(kvp.Key + "," + kvp.Value);

            }
            
        }
    }
}
