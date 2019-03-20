using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace NavneDataXls
{
    class Program
    {
        const string url = "https://ast.dk/_namesdb/export/names?format=xls&gendermask=";
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage namesF = client.GetAsync(url + 1).Result; // List of female names
                HttpResponseMessage namesM = client.GetAsync(url + 2).Result; // List of male names
                // HttpResponseMessage namesAll = client.GetAsync(url + 2).Result; // List of all names
                HttpResponseMessage namesU = client.GetAsync(url + 4).Result; // List of unisex names
                List<Stream> streams = new List<Stream>()
                {
                    namesF.Content.ReadAsStreamAsync().Result,
                    namesM.Content.ReadAsStreamAsync().Result,
                    namesU.Content.ReadAsStreamAsync().Result
                };

                foreach (var stream in streams)
                {
                    var sheet = new HSSFWorkbook(stream).GetSheetAt(0); // Old excel format (xls)
                    // var sheet = new XSSFWorkbook(stream).GetSheetAt(0); // New excel format (xlsx)

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        row.ToList().ForEach(System.Console.WriteLine);
                    }
                }
            }
        }
    }
}
