// See https://aka.ms/new-console-template for more information
using TransactionExtractor;
while (true)
{
    Console.WriteLine("Enter records file path:");
    string filepath = Console.ReadLine() ?? "";
    try
    {
        List<TransactionEntry> list = FileProcessor.GetEntries(filepath);
        FileProcessor.WriteEntries(filepath, list);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
        Console.WriteLine(e.Message.ToString());
        Console.Write(e.StackTrace);
    }
}