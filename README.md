What Project Does:  
  This project is a command line application which extracts transaction data from bank statement text files and formats it into a CSV for use in a personal ledger spreadsheet.    
  It namely breaks up each transaction, saving useful pieces, adds some extra data based on said peices, then formats each piece as a csv field.  

  
How Project Works:  

  The program loop starts with asking user for file path for the bank statement text files.  
  A static file processor class then loads the file, and begins constructing TransactionEntry objects with 1 or 2 lines corresponding to a single transaction.  
  These objects are put into a list and passed back to the main loop  
    
  The TransactionEntry object constructs itself by processing the strings passed to it with regex.  
  Some data is perfect as is (dates and transaction amounts), some is discarded, and some is used for decision making (medium, amount sign).  
  A major decision tree based on the medium-of-transaction determines the description, organization, and category.  
  Amount signage also determines which accounts they go to and from.  
  Most of this data is hardcoded for now as most of thsese have few entries and are very consistent.  
  However, card entries are the bulk of tranactions and span a variety of Categories and Orgs.  
    
  So for card entries, the organization is extracted from the second line with regrex due to extraneous infomration and inconsistent formatting.  
  These organizations are then used to assume the category for the bulk of entries (e.g. a restraurant is clearly an external-food Category)  
    (The extracted organizations are notably consistent across same orgs as all unique information is discarded, only different POS systems have led to a couple duplicates)  
    (This means orgs can be used as unique keys in a dictionary for Category assignment)  
  This is performed with an organization-category dictionary which is loaded-from/saved-to disk (local to the .exe).  
  This saved dictionary does not initally contain meaningful values in the KVPs (they start with default values) until they are manually edited.    
    (Category correspondance is not contained within the transaction data, but rather a personal attribution, requiring manual data entry)  
  As a result, the program needs two run throughs, once to fill the save file with all organizations (dictionary keys).  
  The second, after manually editting the org-cat file, to add accurate category data to the transactions.  
    
  Back at the main loop, a static write to CSV method is called, passing the list of transaction objets  
  The output file path is basically the input path & name but with csv instead of txt appended.  
  This will overwrite any file of that name.  
  The filepath is written to the console for convenience.  
  The write method then loops through, calling a ToCSV method for each and writing to disk.  
  The loop then returns to the start, asking for another file.  

  
Why Project Useful:  

  This let me add a significant amount of transactions that I was not tracking previously to my personal ledger.  
  Now I have a much longer history to compare against for budgeting.  
  This also could be forked and used as a foundation for future simple reformatting projects.  

  
Possible Future Project Features:  

  Fork with all personal data scrambled for public release
  More robust and feature-rich file I/O  
    Allow multiple file inputs at one time  
    Options for combining multiple files into one  
    give option for mid-process dictionary/descriptive file editting so two run-throughs isn't necessary  
    option to select descriptive/dictionary file, allowing for easy management and swapping  
  Move all descriptive data out of hard code and into save file for easy personalization or editing  
    data includes description, organization, category, and accounts  
    likely use JSON or similar to allow customizable decision tree  
  More robust error handling; right now it's minimal as I just needed it to function, and if I break something that's okay  
  
  Maybe a more dynamic formmating system entirely?  based entirely on one or more formatting files?  hmmm...  
    
  
  
