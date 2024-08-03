using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransactionExtractor
{
    internal static class FileProcessor
    {

        

        public static List<TransactionEntry> GetEntries(string file)
        {

            List<TransactionEntry> list = new();

            using (StreamReader sr = new(
                new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read) ) )
            {
                while(sr.Peek() != -1)
                {
                    string firstLine = sr.ReadLine() ?? "";
                    string secondLine = "";
                    if ( !TransactionEntry.IsSingleLineGeneratedRegex().IsMatch(firstLine) )
                    {
                        secondLine = sr.ReadLine() ?? "";
                    }

                    list.Add(new TransactionEntry(firstLine, secondLine));

                }

                
            }

            return list;
        }

        public static void WriteEntries(string file, List<TransactionEntry> entries)
        {
            using (StreamWriter sw = new(
                new FileStream(file, FileMode.Create, FileAccess.Write)))
            {
                foreach (var entry in entries)
                {
                    sw.WriteLine(entry.ToCSVLine());
                    
                }
                sw.WriteLine("testing");
            }
        }
    }
}
