using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransactionExtractor
{
    internal static partial class FileProcessor
    {

        [GeneratedRegex(@"[\\/]?(\w+\.\w+$)")]
        public static partial Regex pathFileNameGR();

        const string outputFile = "\\output.csv";


        /// <summary>
        /// Gets entries from a file path and constructs them into a list of entry objects
        /// </summary>
        /// <param name="file">file to extract from</param>
        /// <returns>list of transaction entry objects</returns>
        public static List<TransactionEntry> GetEntries(string file)
        {

            List<TransactionEntry> list = new();

            using (StreamReader sr = new(
                new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read) ) )
            {
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


            }

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
            string newFile = pathFileNameGR().Replace(file, outputFile);
            using (StreamWriter sw = new(
                new FileStream(newFile, FileMode.Create, FileAccess.Write)))
            {
                foreach (var entry in entries)
                {
                    sw.Write(entry.ToCSVLine());
                    
                }
            }
        }
    }
}
