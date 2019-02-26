using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace Dict
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            // Download dictionary from http://www.stavekontrolden.dk/
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile("http://www.stavekontrolden.dk/main/sspinputfiles/da_DK.dic", Environment.CurrentDirectory + "/Words-DK.txt");
            }

            stopWatch.Stop();
            Console.WriteLine($"Download took a total of {stopWatch.Elapsed.Seconds} seconds");


            // Variables
            var wordList = File.ReadAllLines(Environment.CurrentDirectory + "/Words-DK.txt");
            var filtered = new List<string>();
            var slashOccurrences = 0;
            var junk = 0;
            var word = "";
            var outputFile = Environment.CurrentDirectory + "/FilteredList.txt";

            stopWatch.Restart();
            // Loop to clean data
            foreach (var w in wordList)
            {
                if (w.Contains("/"))
                {
                    word = w.Split("/")[0];
                    slashOccurrences++;
                }
                if (string.IsNullOrEmpty(word))
                {
                    junk++;
                    continue;
                }

                bool isNumeric = int.TryParse(word[0].ToString(), out int n);

                if ((word.Length < 2 && word.ToLower() != "å") ||
                word[0] == '"' || word[0] == '-' || isNumeric ||
                word.Last() == '-' || word.Last().ToString() == "'")
                {
                    junk++;
                    continue;
                }

                // Make all words TitleCase and add them to the "filtered" list
                word = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower());
                filtered.Add(word);
            }

            // Remove duplicates and sort alphabetically
            int beforeDuplicates = filtered.Count();
            filtered = filtered.Distinct().ToList();
            filtered.Sort();
            int duplicates = beforeDuplicates - filtered.Count();
            stopWatch.Stop();
            Console.WriteLine($"Filtering the data took a total of {stopWatch.Elapsed.Seconds} Seconds and {stopWatch.Elapsed.Milliseconds} Milliseconds");

            // Print statistics
            Console.WriteLine($"Removed a total of {slashOccurrences} '/', a total of {junk} junk strings and a total of {duplicates} duplicats");

            // Write clean data to new file
            File.WriteAllLines(outputFile, filtered);
        }
    }
}
