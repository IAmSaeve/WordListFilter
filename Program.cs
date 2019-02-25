using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Globalization;

namespace Dict
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists(Environment.CurrentDirectory + "/Words-DK.txt"))
            {
                File.Delete(Environment.CurrentDirectory + "/Words-DK.txt");
            }

            using (WebClient myWebClient = new WebClient())
            {
                myWebClient.DownloadFile("http://www.stavekontrolden.dk/main/sspinputfiles/da_DK.dic", Environment.CurrentDirectory + "/Words-DK.txt");
            }

            var wordList = File.ReadAllLines(Environment.CurrentDirectory + "/Words-DK.txt");
            var filtered = new List<string>();
            var slashOccurrences = 0;
            var junk = 0;
            var word = "";
            var outputFile = Environment.CurrentDirectory + "/FilteredList.txt";

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

                word = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower());
                filtered.Add(word);
            }

            filtered = filtered.Distinct().ToList();
            filtered.Sort();

            Console.WriteLine($"Removed a total of {slashOccurrences} '/' and a total of {junk} junk strings");

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
                File.Create(outputFile).Dispose();
            }
            else
            {
                File.Create(outputFile).Dispose();
            }

            File.WriteAllLines(outputFile, filtered);
        }
    }
}
