using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TransactionExtractor.TransactionEntry;

namespace TransactionExtractor
{
    internal static class OrgToCatDictionary
    {

        private static Dictionary<string, TransactionEntry.Category> dictionary = FileProcessor.GetSavedOrgs();

        /// <summary>
        /// Updates the dictionary from file
        /// </summary>
        internal static void UpdateDictionary ()
        {
            dictionary = FileProcessor.GetSavedOrgs();
        }

        /// <summary>
        /// Gets the category for a given org
        /// adds it to the dictionary if it doesn't already exist
        /// then saves the dictionary if dictionary is updated
        /// </summary>
        /// <param name="org">org to get category for</param>
        /// <returns>Corresponding category for an org</returns>
        internal static Category GetCatFromOrgAndUpdate(string org)
        {

            if (!dictionary.TryGetValue(org, out Category cat))
            {
                dictionary.Add(org, Category.Unknown);

                FileProcessor.SaveOrgsAndCats(dictionary);
            }
            return cat;
        }

        /// <summary>
        /// Gets the category for a given org if it exists
        /// else returns unknown
        /// </summary>
        /// <param name="org">org to get category for</param>
        /// <returns>corresponding category for an org</returns>
        internal static Category GetCatFromOrg(string org)
        {
            if (dictionary.TryGetValue(org,out Category cat))
                return cat;
            else 
                return Category.Unknown;
        }


    }
}
