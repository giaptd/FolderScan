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

                var finalResultFolder = ConfigurationManager.AppSettings["finalResultFolder"];
                if (!Directory.Exists(finalResultFolder))
                {
                    Directory.CreateDirectory(finalResultFolder);
                }

                var fileNames = Directory.EnumerateFiles(sourceFolderPath,"*.zip", SearchOption.AllDirectories);

                //2. read source CSV file
                List<EmpInfo> records;
                using (var reader = new StreamReader(sourceCSVFile))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.HasHeaderRecord = true;
                        records = csv.GetRecords<EmpInfo>().ToList();
                    }
                }
                //Console.WriteLine(records.ToList().Count);
                
                // 3.
                foreach(var file in fileNames)
                {
                    int fNumber;
                    // convert file name to INT
                    var fileNameOnly = Path.GetFileNameWithoutExtension(file);
                    var fileExt = Path.GetExtension(file);
                    bool fIsNumber = int.TryParse(fileNameOnly, out fNumber);
                    if (fIsNumber)
                    {
                        var matchRecord = records.FirstOrDefault(r => r.ID == fNumber);
                        if (matchRecord != null)
                        {
                            // found match record
                            // first rename the file
                            var newName = $"{matchRecord.LN}_{matchRecord.FN}_{matchRecord.ID}{fileExt}";
                            File.Move(file, Path.Combine(finalResultFolder, newName));
                            // Remove match item from list
                            records.Remove(matchRecord);
                        }
                    }
                }

                // dump records to a new file
                using (var writer = new StreamWriter(Path.Combine(finalResultFolder, "Remaining.csv")))
                {
                    using (var csvWriter = new CsvWriter(writer))
                    {
                        csvWriter.Configuration.HasHeaderRecord = true;
                        csvWriter.WriteRecords(records);
                    }
                }

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
        public int ID { get; set; }
        public string FN { get; set; }
        public string LN { get; set; }
    }
}
