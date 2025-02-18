// See https://aka.ms/new-console-template for more information
using TransactionExtractor;


//Main command input loop
while (true)
{

    Console.Write("Transaction Extractor > ");
    string input = Console.ReadLine() ?? "";

    try
    {
        CommandProcessor.ProcessCommand(input);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.WriteLine(e.Message.ToString());
        Console.Write(e.StackTrace);
    }




}