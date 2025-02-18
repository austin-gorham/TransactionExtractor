using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using static TransactionExtractor.TransactionEntry;

namespace TransactionExtractor
{
    internal static class FileProcessor
    {

        //Organization and Category dictionary file
        const string orgFile = "OrgsAndCats.txt";


        /// <summary>
        /// Gets entries from a file path and constructs them into a list of entry objects
        /// </summary>
        /// <param name="file">file to extract from</param>
        /// <returns>list of transaction entry objects</returns>
        internal static List<TransactionEntry> GetEntriesFromTxt(string file)
        {

            List<TransactionEntry> list = [];

            using StreamReader sr = new(
                new FileStream(file, FileMode.Open, FileAccess.Read));

            /**
             * Entries can be 1 or 2 lines
             * so need to queue up two at a time
             * and check if they are both entries
             * before creating an entry out of the first line
             * tags the second line along if it is not an entry (assumed part of first)
             */
            string newEntry = "";
            while (sr.Peek() != -1)
            {

                /** Loop logic:
                 * If the next line is an entry's first line
                     * create object from previous entry if not empty,
                     * and assign next line to new entry for next go around
                     * (need to check following line if part of newEntry)
                 * else if next line is not a new entry and previous entry exists
                     * create object from newEntry and nextLine
                     * (no validation of second line; assumed part of first)
                     * set newEntry to empty as it's been used
                 * After loop exit, last entry could still be in newEntry
                 * as there was no nextLine to check and loop exits before logic of said check
                 * so if not empty, create object from newEntry
                 */

                string nextLine = sr.ReadLine()?.Trim() ?? "";

                if ( RegexContainer.IsNewEntryGR().IsMatch(nextLine) )
                {
                    if (!String.IsNullOrEmpty(newEntry))
                        list.Add(new TransactionEntry(newEntry));
                    newEntry = nextLine;
                } else if (!String.IsNullOrEmpty(newEntry)) //i think unnecessary if input is reliable, not sure if i want to keep error handing here or pass on
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
        internal static void WriteEntries(string file, List<TransactionEntry> entries)
        {
            Console.WriteLine(file);
            string outputFile;

            if (RegexContainer.FileExtensionGR().Match(file.TrimEnd()).Success)
                outputFile = RegexContainer.FileExtensionGR().Replace(file, ".csv");
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
        internal static Dictionary<string, TransactionEntry.Category> GetSavedOrgs()
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
                    try //trys to parse enum, adds with default value if it fails
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
        internal static void SaveOrgsAndCats(Dictionary<string, TransactionEntry.Category> dict)
        {
            using StreamWriter sw = new(
                new FileStream(orgFile, FileMode.Create, FileAccess.Write));

            foreach (var kvp in dict)
            {
                sw.WriteLine(kvp.Key + "," + kvp.Value);

            }
            
        }


        internal static List<TransactionEntry> GetEntriesFromPDF(string pdfFile)
        {
            List<TransactionEntry> list = [];

            using (PdfDocument document = PdfDocument.Open(pdfFile))
            {
                string newEntry = "";
                foreach (Page page in document.GetPages())
                {
                    var words = page.GetWords();

                    var blocks = DefaultPageSegmenter.Instance.GetBlocks(words);

                    bool inDataBlock = false;

                    
                    foreach (var block in blocks)
                    {
                        
                        foreach (TextLine line in block.TextLines)
                        {
                            if(inDataBlock)
                            {
                                if (inDataBlock 
                                    && RegexContainer.IsDataBlockExitGR().Match(line.ToString()).Success) 
                                {
                                    inDataBlock = false;
                                    break;
                                }
                                string nextLine = line.ToString();

                                if (RegexContainer.IsNewEntryGR().IsMatch(nextLine))
                                {
                                    //Console.WriteLine(line.ToString());
                                    if (!String.IsNullOrEmpty(newEntry))
                                        list.Add(new TransactionEntry(newEntry));

                                    newEntry = nextLine;
                                }
                                else if (!String.IsNullOrEmpty(newEntry)) //i think unnecessary if input is reliable, not sure if i want to keep error handing here or pass on
                                {

                                    list.Add(new TransactionEntry(newEntry, nextLine));
                                    newEntry = String.Empty;
                                }
                                //Console.WriteLine(line.ToString());
                            } else
                            {
                                if (!inDataBlock 
                                    && RegexContainer.IsDataBlockEntranceGR().Match(line.ToString()).Success)
                                    inDataBlock = true;
                            }
                            //Console.WriteLine(line.ToString());
                        }

                    }
                }
                if (!String.IsNullOrEmpty(newEntry))
                    list.Add(new TransactionEntry(newEntry));
            }
            return list;
        }
    }
}
