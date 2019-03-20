using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace WithLINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            // Variables
            List<string> wordList = new List<string>();
            var filtered = new List<string>();
            var junk = 0;
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
            filtered = wordList
                        .Select(w => w.Split("/")[0])
                        .Where(w =>
                            !CheckSingleLetter(w) && CheckQoutes(w) && CheckFirstDash(w) &&
                            !IsNumeric(w) && CheckLastDash(w) && CheckApostrophe(w) &&
                            CheckLinesWithSpace(w) && CheckDot(w)
                        )
                        .Select(w => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(w.ToLower()))
                        .Distinct()
                        .ToList();
            filtered.Sort();

            // Stop counter and print elapsed time
            stopWatch.Stop();
            Console.WriteLine($"Filtering the data took a total of {stopWatch.Elapsed.Seconds} Seconds and {stopWatch.Elapsed.Milliseconds} Milliseconds");

            junk = wordList.Count() - filtered.Count();

            // Print statistics
            Console.WriteLine($"Removed a total of {junk} junk data");

            // Write clean data to new file
            File.WriteAllLines(outputFile, filtered);
        }

        private static bool CheckDot(string w) => !w.Contains(".");
        private static bool CheckLinesWithSpace(string w) => !w.Contains(" ");
        private static bool CheckApostrophe(string w) => !w.EndsWith("'");
        private static bool CheckLastDash(string w) => !w.EndsWith('-');
        private static bool IsNumeric(string w) => int.TryParse(w[0].ToString(), out int n);
        private static bool CheckFirstDash(string w) => !w.StartsWith('-');
        private static bool CheckQoutes(string w) => !w.StartsWith('"');
        private static bool CheckSingleLetter(string w) => (w.Length < 2 && w.ToLower() != "å");
    }
}
