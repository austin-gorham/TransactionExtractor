using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransactionExtractor
{
    public static partial class CommandProcessor
    {
        [GeneratedRegex(@"(\w+)(\s.*)?", RegexOptions.IgnoreCase)]
        public static partial Regex CommandGR();

        private static List<TransactionEntry> transactionList = new();

        /// <summary>
        /// Initial command parsing and decision tree
        /// </summary>
        /// <param name="input">command to parse</param>
        public static void ProcessCommand(string input) { 
            Match commandMatch = CommandGR().Match(input); 

            string command = commandMatch.Groups[1].Value;
            string arguments = commandMatch.Groups[2].Value;

            Console.WriteLine("command entered: " + command);
            Console.WriteLine("arguments entered: " + arguments);

            switch (command) 
            {
                case "import": 
                    ImportFromPaths(arguments);
                    break;
                case "export":
                    ExportEntries(arguments);
                    break;
                case "update":
                    UpdateEntries();
                    break;
                case "help":
                    WriteHelpText();
                    break;
                default:
                    Console.WriteLine("Unknown command.  Use command 'help' for assistance.");
                    break;
            }
        }

        /// <summary>
        /// Parses the import command arguments and imports files
        /// splits the path arguments by spaces to seperate out multiple filepaths
        /// iterates through each
        /// simple try/catch for success/fail handling
        /// Adds the new entry list to current list
        /// writes out which paths succeed and which fail
        /// </summary>
        /// <param name="pathsArgument">arguments (filepaths) from import command</param>
        private static void ImportFromPaths(string pathsArgument)
        {
            string[] paths = pathsArgument.Trim().Split(new char[] { ' ' });

            foreach(string path in paths)
            {
                try
                {
                    transactionList.AddRange(FileProcessor.GetEntries(path.Trim('"','\'')));
                    Console.WriteLine(" > Successfully imported " + path);
                }
                catch 
                {
                    Console.WriteLine(" > Failed to import " + path);
                }

            }
        }

        /// <summary>
        /// updates all entries with new categories
        /// if categories have changed
        /// </summary>
        private static void UpdateEntries()
        {
            foreach (TransactionEntry entry in transactionList)
            {
                entry.UpdateUnknownCat();
            }
        }

        /// <summary>
        /// Uses the file processor to write
        /// current list to specified file
        /// </summary>
        /// <param name="pathArgument">file to write to</param>
        private static void ExportEntries(string pathArgument)
        {
            FileProcessor.WriteEntries(pathArgument, transactionList);
        }

        /// <summary>
        /// Write help text to console
        /// </summary>
        private static void WriteHelpText()
        {
            Console.WriteLine(" > Commands :");
            Console.WriteLine(" > import <filepath> : imports files and adds entries to current list");
            Console.WriteLine(" > export <filepath> : exports current entries to filepath");
            Console.WriteLine(" > update : updates current entries with any recent changes to OrgsAndCats.txt");
        }
    }
}
