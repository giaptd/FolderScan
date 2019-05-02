using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace MySample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Scan all folders into an array => a1
            // 2. read source csv file => s1
            // for each item in a1, if found in s1
            // Rename folder item follow information from s1
            // Remove item from s1

            try
            {
                // 1. Get all file names in source folder
                var sourceFolderPath = ConfigurationManager.AppSettings["rootFolder"];
                if (!Directory.Exists(sourceFolderPath))
                {
                    Console.WriteLine("Source folder does not exist");
                    return;
                }
                var sourceCSVFile = ConfigurationManager.AppSettings["sourceCsvFile"];
                if (!File.Exists(sourceCSVFile))
                {
                    Console.WriteLine("Source CSV file does not exist");
                    return;
                }

                var fileNames = Directory.EnumerateFiles(sourceFolderPath);

                //2. read source CSV file
                IEnumerable<EmpInfo> records;
                using (var reader = new StreamReader(sourceCSVFile))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        records = csv.GetRecords<EmpInfo>();
                    }
                }
                Console.WriteLine(records.Count());
                
                //foreach (var fileName in fileNames)
                //{
                //    Console.WriteLine(fileName);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            // stop
            Console.WriteLine("press any key to exit");
            Console.ReadLine();
        }
    }

    public class EmpInfo
    {
        public string ID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
    }
}
