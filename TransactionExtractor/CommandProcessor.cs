﻿using System.Text.RegularExpressions;

namespace TransactionExtractor
{
    internal static class CommandProcessor
    {

        private static readonly List<TransactionEntry> transactionList = [];

        /// <summary>
        /// Initial command parsing and decision tree
        /// </summary>
        /// <param name="input">command to parse</param>
        internal static void ProcessCommand(string input)
        {
            Match commandMatch = RegexContainer.CommandGR().Match(input);

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
            string[] paths = pathsArgument.Trim().Split(' ');


            foreach (string path in paths)
            {
                try
                {
                    string trimmedPath = path.Trim('"', '\'', ' ');
                    string extension = RegexContainer.FileExtensionGR().Match(trimmedPath).Groups[1].Value;

                    switch (extension)
                    {
                        case ".txt":
                            transactionList.AddRange(FileProcessor.GetEntriesFromTxt(trimmedPath));
                            Console.WriteLine(" > Successfully imported " + trimmedPath);
                            break;
                        case ".pdf":
                            transactionList.AddRange(FileProcessor.GetEntriesFromPDF(trimmedPath));
                            Console.WriteLine(" > Successfully imported " + trimmedPath);
                            break;
                        default:
                            Console.WriteLine($" > Unsupported file extension '{extension}'");
                            break;
                    }

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
            FileProcessor.WriteEntries(pathArgument.Trim('"', '\'', ' '), transactionList);
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
