using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace WithForEach
{
    class Program
    {
        static void Main(string[] args)
        {
            // Variables
            List<string> wordList = new List<string>();
            var filtered = new List<string>();
            var junk = 0;
            var slashOccurrences = 0;
            var word = "";
            var outputFile = Environment.CurrentDirectory + "/FilteredList.txt";
            var uri = "http://www.stavekontrolden.dk/main/sspinputfiles/da_DK.dic";
            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            // Download dictionary from http://www.stavekontrolden.dk/
            using (WebClient wc = new WebClient())
            {
                wordList = new List<string>((wc.DownloadString(uri)).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            }

            // Stop counter and print elapsed time
            stopWatch.Stop();
            Console.WriteLine($"Download took a total of {stopWatch.Elapsed.Seconds} seconds and {stopWatch.Elapsed.Milliseconds} Milliseconds");
            stopWatch.Restart();

            // Loop to clean data
            foreach (var w in wordList)
            {
                if (w.Contains("/"))
                {
                    word = w.Split("/")[0];
                    slashOccurrences++;
                }

                if (CheckSingleLetter(word) || CheckQoutes(word) || CheckFirstDash(word) ||
                    IsNumeric(word) || CheckLastDash(word) || CheckApostrophe(word))
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

            // Stop counter and print elapsed time
            stopWatch.Stop();
            Console.WriteLine($"Filtering the data took a total of {stopWatch.Elapsed.Seconds} Seconds and {stopWatch.Elapsed.Milliseconds} Milliseconds");

            // Print statistics
            Console.WriteLine($"Removed a total of {slashOccurrences} '/', a total of {junk} junk strings and a total of {duplicates} duplicats");

            // Write clean data to new file
            File.WriteAllLines(outputFile, filtered);
        }

        private static bool CheckApostrophe(string w) => w.Last().ToString() == "'";
        private static bool CheckLastDash(string w) => w.Last() == '-';
        private static bool IsNumeric(string w) => int.TryParse(w[0].ToString(), out int n);
        private static bool CheckFirstDash(string w) => w[0] == '-';
        private static bool CheckQoutes(string w) => w[0] == '"';
        private static bool CheckSingleLetter(string w) => (w.Length < 2 && w.ToLower() != "å");
    }
}
